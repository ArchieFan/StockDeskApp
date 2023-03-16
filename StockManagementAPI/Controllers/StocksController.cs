namespace StockDesk.StockManagement.Controllers;

[Route("/api/[controller]")]
public class StocksController : Controller
{
    //private const string NUMBER_PATTERN = @"^((\d{1,3}|[a-z]{1,3})-){2}(\d{1,3}|[a-z]{1,3})$";
    IMessagePublisher _messagePublisher;
    StockManagementDBContext _dbContext;

    public StocksController(StockManagementDBContext dbContext, IMessagePublisher messagePublisher)
    {
        _dbContext = dbContext;
        _messagePublisher = messagePublisher;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _dbContext.Stocks.ToListAsync());
    }

    [HttpGet]
    [Route("{ticker}", Name = "GetByTicker")]
    public async Task<IActionResult> GetByTicker(string ticker)
    {
        var stock = await _dbContext.Stocks.FirstOrDefaultAsync(v => v.Ticker == ticker);
        if (stock == null)
        {
            return NotFound();
        }
        return Ok(stock);
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterStock command)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // check invariants
                //if (!Regex.IsMatch(command.Ticker, NUMBER_PATTERN, RegexOptions.IgnoreCase))
                //{
                //    return BadRequest($"The specified license-number '{command.Ticker}' was not in the correct format.");
                //}

                // insert Stock
                Stock stock = command.MapToStock();
                _dbContext.Stocks.Add(stock);
                await _dbContext.SaveChangesAsync();

                // send event
                var e = StockRegistered.FromCommand(command);
                await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");

                //return result
                return CreatedAtRoute("GetByTicker", new { ticker = stock.Ticker }, stock);
            }
            return BadRequest();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Unable to save changes. " +
                "Try again, and if the problem persists " +
                "see your system administrator.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
