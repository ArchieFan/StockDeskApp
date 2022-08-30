namespace StockDesk.WebApp.Controllers;

public class ClientManagementController : Controller
{
    private readonly IClientManagementAPI _clientManagementAPI;
    private readonly Microsoft.Extensions.Logging.ILogger _logger;
    private ResiliencyHelper _resiliencyHelper;

    public ClientManagementController(IClientManagementAPI clientManagementAPI, ILogger<ClientManagementController> logger)
    {
        _clientManagementAPI = clientManagementAPI;
        _logger = logger;
        _resiliencyHelper = new ResiliencyHelper(_logger);
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return await _resiliencyHelper.ExecuteResilient(async () =>
        {
            var model = new ClientManagementViewModel
            {
                Clients = await _clientManagementAPI.GetClients()
            };
            return View(model);
        }, View("Offline", new ClientManagementOfflineViewModel()));
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        return await _resiliencyHelper.ExecuteResilient(async () =>
        {
            var model = new ClientManagementDetailsViewModel
            {
                Client = await _clientManagementAPI.GetClientById(id)
            };
            return View(model);
        }, View("Offline", new ClientManagementOfflineViewModel()));
    }

    [HttpGet]
    public IActionResult New()
    {
        var model = new ClientManagementNewViewModel
        {
            Client = new Client()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromForm] ClientManagementNewViewModel inputModel)
    {
        if (ModelState.IsValid)
        {
            return await _resiliencyHelper.ExecuteResilient(async () =>
            {
                RegisterClient cmd = inputModel.MapToRegisterClient();
                await _clientManagementAPI.RegisterClient(cmd);
                return RedirectToAction("Index");
            }, View("Offline", new ClientManagementOfflineViewModel()));
        }
        else
        {
            return View("New", inputModel);
        }
    }
}