﻿namespace StockDesk.PortfolioManagementAPI.Controllers;

[Route("/api/[controller]")]
public class PortfolioPlanningController : Controller
{
    private readonly IEventSourceRepository<PortfolioPlanning> _planningRepo;
    private readonly IPlanTradingCommandHandler _planTradingCommandHandler;
    private readonly IFinishTradingCommandHandler _finishTradingCommandHandler;

    public PortfolioPlanningController(
        IEventSourceRepository<PortfolioPlanning> planningRepo,
        IPlanTradingCommandHandler planTradingCommandHandler,
        IFinishTradingCommandHandler finishTradingCommand)
    {
        _planningRepo = planningRepo;
        _planTradingCommandHandler = planTradingCommandHandler;
        _finishTradingCommandHandler = finishTradingCommand;
    }

    [HttpGet]
    [Route("{planningDate}", Name = "GetByDate")]
    public async Task<IActionResult> GetByDate(DateTime planningDate)
    {
        try
        {
            var aggregateId = PortfolioPlanningId.Create(planningDate);
            var planning = await _planningRepo.GetByIdAsync(aggregateId);
            if (planning == null)
            {
                return NotFound();
            }

            return Ok(planning.MapToDTO());
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
            throw;
        }
    }

    [HttpGet]
    [Route("{planningDate}/jobs/{jobId}")]
    public async Task<IActionResult> GetTradingAsync(DateTime planningDate, Guid jobId)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // get planning
                var aggregateId = PortfolioPlanningId.Create(planningDate);
                var planning = await _planningRepo.GetByIdAsync(aggregateId);
                if (planning == null || planning.Jobs == null)
                {
                    return NotFound();
                }
                // get job
                var job = planning.Jobs.FirstOrDefault(j => j.Id == jobId);
                if (job == null)
                {
                    return NotFound();
                }
                return Ok(job.MapToDTO());
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        return BadRequest();
    }

    [HttpPost]
    [Route("{planningDate}/jobs")]
    public async Task<IActionResult> PlanTradingAsync(DateTime planningDate, [FromBody] PlanTrading command)
    {
        try
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // handle command
                    PortfolioPlanning planning = await
                        _planTradingCommandHandler.HandleCommandAsync(planningDate, command);

                    // handle result    
                    if (planning == null)
                    {
                        return NotFound();
                    }

                    // return result
                    return CreatedAtRoute("GetByDate", new { planningDate = planning.Id }, planning.MapToDTO());
                }
                catch (BusinessRuleViolationException ex)
                {
                    return StatusCode(StatusCodes.Status409Conflict, new BusinessRuleViolation { ErrorMessage = ex.Message });
                }
            }
            return BadRequest();
        }
        catch (ConcurrencyException)
        {
            string errorMessage = "Unable to save changes. " +
                "Try again, and if the problem persists " +
                "see your system administrator.";
            Log.Error(errorMessage);
            ModelState.AddModelError("ErrorMessage", errorMessage);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut]
    [Route("{planningDate}/jobs/{jobId}/finish")]
    public async Task<IActionResult> FinishTradingAsync(DateTime planningDate, Guid jobId, [FromBody] FinishTrading command)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // handle command
                PortfolioPlanning planning = await
                    _finishTradingCommandHandler.HandleCommandAsync(planningDate, command);

                // handle result    
                if (planning == null)
                {
                    return NotFound();
                }

                // return result
                return Ok();
            }
            return BadRequest();
        }
        catch (ConcurrencyException)
        {
            string errorMessage = "Unable to save changes. " +
                "Try again, and if the problem persists " +
                "see your system administrator.";
            Log.Error(errorMessage);
            ModelState.AddModelError("", errorMessage);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}