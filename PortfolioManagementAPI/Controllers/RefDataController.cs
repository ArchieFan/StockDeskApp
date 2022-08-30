namespace StockDesk.PortfolioManagementAPI.Controllers;

[Route("/api/[controller]")]
public class RefDataController : Controller
{
    IClientRepository _clientRepo;
    IStockRepository _stockRepo;

    public RefDataController(IClientRepository clientRepo, IStockRepository stockRepo)
    {
        _clientRepo = clientRepo;
        _stockRepo = stockRepo;
    }

    [HttpGet]
    [Route("clients")]
    public async Task<IActionResult> GetClients()
    {
        return Ok(await _clientRepo.GetClientsAsync());
    }

    [HttpGet]
    [Route("clients/{clientId}")]
    public async Task<IActionResult> GetClientByClientId(string clientId)
    {
        var client = await _clientRepo.GetClientAsync(clientId);
        if (client == null)
        {
            return NotFound();
        }
        return Ok(client);
    }

    [HttpGet]
    [Route("stocks")]
    public async Task<IActionResult> GetStocks()
    {
        return Ok(await _stockRepo.GetStocksAsync());
    }

    [HttpGet]
    [Route("stocks/{ticker}")]
    public async Task<IActionResult> GetStockByTicker(string ticker)
    {
        var stock = await _stockRepo.GetStockAsync(ticker);
        if (stock == null)
        {
            return NotFound();
        }
        return Ok(stock);
    }
}