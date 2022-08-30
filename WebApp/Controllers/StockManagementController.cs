namespace StockDesk.WebApp.Controllers;

    public class StockManagementController : Controller
{
    private IStockManagementAPI _stockManagementAPI;
    private IClientManagementAPI _clientManagementAPI;
    private readonly Microsoft.Extensions.Logging.ILogger _logger;
    private ResiliencyHelper _resiliencyHelper;

    public StockManagementController(IStockManagementAPI stockManagementAPI,
        IClientManagementAPI clientManagementAPI, ILogger<StockManagementController> logger)
    {
        _stockManagementAPI = stockManagementAPI;
        _clientManagementAPI = clientManagementAPI;
        _logger = logger;
        _resiliencyHelper = new ResiliencyHelper(_logger);
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return await _resiliencyHelper.ExecuteResilient(async () =>
        {
            var model = new StockManagementViewModel
            {
                Stocks = await _stockManagementAPI.GetStocks()
            };
            return View(model);
        }, View("Offline", new StockManagementOfflineViewModel()));
    }

    [HttpGet]
    public async Task<IActionResult> Details(string ticker)
    {
        return await _resiliencyHelper.ExecuteResilient(async () =>
        {
            Stock stock = await _stockManagementAPI.GetStockByTicker(ticker);
            Client client = await _clientManagementAPI.GetClientById(stock.OwnerId);

            var model = new StockManagementDetailsViewModel
            {
                Stock = stock,
                Owner = client.Name
            };
            return View(model);
        }, View("Offline", new StockManagementOfflineViewModel()));
    }

    [HttpGet]
    public async Task<IActionResult> New()
    {
        return await _resiliencyHelper.ExecuteResilient(async () =>
        {
                // get clientlist
                var clients = await _clientManagementAPI.GetClients();

            var model = new StockManagementNewViewModel
            {
                Stock = new Stock(),
                Clients = clients.Select(c => new SelectListItem { Value = c.ClientId, Text = c.Name })
            };
            return View(model);
        }, View("Offline", new StockManagementOfflineViewModel()));
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromForm] StockManagementNewViewModel inputModel)
    {
        if (ModelState.IsValid)
        {
            return await _resiliencyHelper.ExecuteResilient(async () =>
            {
                RegisterStock cmd = inputModel.MapToRegisterStock();
                await _stockManagementAPI.RegisterStock(cmd);
                return RedirectToAction("Index");
            }, View("Offline", new StockManagementOfflineViewModel()));
        }
        else
        {
            return View("New", inputModel);
        }
    }

    [HttpGet]
    public IActionResult Error()
    {
        return View();
    }
}