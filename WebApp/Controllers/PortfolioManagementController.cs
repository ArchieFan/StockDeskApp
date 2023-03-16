namespace StockDesk.WebApp.Controllers;

    public class PortfolioManagementController : Controller
{
    private IPortfolioManagementAPI _portfolioManagementAPI;
    private readonly Microsoft.Extensions.Logging.ILogger _logger;
    private ResiliencyHelper _resiliencyHelper;

    public PortfolioManagementController(IPortfolioManagementAPI portfolioManagamentAPI, ILogger<PortfolioManagementController> logger)
    {
        _portfolioManagementAPI = portfolioManagamentAPI;
        _logger = logger;
        _resiliencyHelper = new ResiliencyHelper(_logger);
    }

    [HttpGet]
    public async Task<IActionResult> Index(DateTime? planningDate)
    {
        return await _resiliencyHelper.ExecuteResilient(async () =>
        {
            if (planningDate == null)
            {
                planningDate = DateTime.Now.Date;
            }

            var model = new PortfolioManagementViewModel
            {
                Date = planningDate.Value,
                Tradings = new List<Trading>()
            };

                // get planning
                string dateStr = planningDate.Value.ToString("yyyy-MM-dd");
            PortfolioPlanning planning = await _portfolioManagementAPI.GetPortfolioPlanning(dateStr);
            if (planning?.Jobs?.Count > 0)
            {
                model.Tradings.AddRange(planning.Jobs.OrderBy(j => j.StartTime));
            }

            return View(model);
        }, View("Offline", new PortfolioManagementOfflineViewModel()));
    }

    [HttpGet]
    public async Task<IActionResult> Details(DateTime planningDate, string jobId)
    {
        return await _resiliencyHelper.ExecuteResilient(async () =>
        {
            string dateStr = planningDate.ToString("yyyy-MM-dd");
            var model = new PortfolioManagementDetailsViewModel
            {
                Date = planningDate,
                Trading = await _portfolioManagementAPI.GetTrading(dateStr, jobId)
            };
            return View(model);
        }, View("Offline", new PortfolioManagementOfflineViewModel()));
    }

    [HttpGet]
    public async Task<IActionResult> New(DateTime planningDate)
    {
        return await _resiliencyHelper.ExecuteResilient(async () =>
        {
            DateTime startTime = planningDate.Date.AddHours(8);

            var model = new PortfolioManagementNewViewModel
            {
                Id = Guid.NewGuid(),
                Date = planningDate,
                StartTime = startTime,
                EndTime = startTime.AddHours(2),
                Stocks = await GetAvailableStocksList()
            };
            return View(model);
        }, View("Offline", new PortfolioManagementOfflineViewModel()));
    }

    [HttpGet]
    public async Task<IActionResult> Finish(DateTime planningDate, string jobId)
    {
        return await _resiliencyHelper.ExecuteResilient(async () =>
        {
            string dateStr = planningDate.ToString("yyyy-MM-dd");
            Trading job = await _portfolioManagementAPI.GetTrading(dateStr, jobId);
            var model = new PortfolioManagementFinishViewModel
            {
                Id = job.Id,
                Date = planningDate,
                ActualStartTime = job.StartTime,
                ActualEndTime = job.EndTime
            };
            return View(model);
        }, View("Offline", new PortfolioManagementOfflineViewModel()));
    }

    [HttpPost]
    public async Task<IActionResult> RegisterTrading([FromForm] PortfolioManagementNewViewModel inputModel)
    {
        if (ModelState.IsValid)
        {
            return await _resiliencyHelper.ExecuteResilient(async () =>
            {
                string dateStr = inputModel.Date.ToString("yyyy-MM-dd");

                try
                {
                        // register trading job
                        DateTime startTime = inputModel.Date.Add(inputModel.StartTime.TimeOfDay);
                    DateTime endTime = inputModel.Date.Add(inputModel.EndTime.TimeOfDay);
                    Stock stock = await _portfolioManagementAPI.GetStockByTicker(inputModel.SelectedStockTicker);
                    Client client = await _portfolioManagementAPI.GetClientById(stock.OwnerId);

                    PlanTrading planTradingCommand = new PlanTrading(Guid.NewGuid(), Guid.NewGuid(), startTime, endTime,
                        (client.ClientId, client.Name, client.TelephoneNumber),
                        (stock.Ticker, stock.CompanyName, stock.Industry, stock.MarketCap), inputModel.Description);
                    await _portfolioManagementAPI.PlanTrading(dateStr, planTradingCommand);
                }
                catch (ApiException ex)
                {
                    if (ex.StatusCode == HttpStatusCode.Conflict)
                    {
                            // add errormessage from API exception to model
                            var content = await ex.GetContentAsAsync<BusinessRuleViolation>();
                        inputModel.Error = content.ErrorMessage;

                            // repopulate list of available stocks in the model
                            inputModel.Stocks = await GetAvailableStocksList();

                            // back to New view
                            return View("New", inputModel);
                    }
                }

                return RedirectToAction("Index", new { planningDate = dateStr });
            }, View("Offline", new PortfolioManagementOfflineViewModel()));
        }
        else
        {
            inputModel.Stocks = await GetAvailableStocksList();
            return View("New", inputModel);
        }
    }

    [HttpPost]
    public async Task<IActionResult> FinishTrading([FromForm] PortfolioManagementFinishViewModel inputModel)
    {
        if (ModelState.IsValid)
        {
            return await _resiliencyHelper.ExecuteResilient(async () =>
            {
                string dateStr = inputModel.Date.ToString("yyyy-MM-dd");
                DateTime actualStartTime = inputModel.Date.Add(inputModel.ActualStartTime.Value.TimeOfDay);
                DateTime actualEndTime = inputModel.Date.Add(inputModel.ActualEndTime.Value.TimeOfDay);

                FinishTrading cmd = new FinishTrading(Guid.NewGuid(), inputModel.Id,
                    actualStartTime, actualEndTime, inputModel.Notes);

                await _portfolioManagementAPI.FinishTrading(dateStr, inputModel.Id.ToString("D"), cmd);

                return RedirectToAction("Details", new { planningDate = dateStr, jobId = inputModel.Id });
            }, View("Offline", new PortfolioManagementOfflineViewModel()));
        }
        else
        {
            return View("Finish", inputModel);
        }
    }

    public IActionResult Error()
    {
        return View();
    }

    private async Task<IEnumerable<SelectListItem>> GetAvailableStocksList()
    {
        var stocks = await _portfolioManagementAPI.GetStocks();
        return stocks.Select(v =>
            new SelectListItem
            {
                Value = v.Ticker,
                Text = $"{v.CompanyName} {v.Industry} [{v.Ticker}]"
            });
    }
}