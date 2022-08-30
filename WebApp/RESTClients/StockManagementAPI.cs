namespace WebApp.RESTClients;

public class StockManagementAPI : IStockManagementAPI
{
    private IStockManagementAPI _restClient;

    public StockManagementAPI(IConfiguration config, HttpClient httpClient)
    {
        string apiHostAndPort = config.GetSection("APIServiceLocations").GetValue<string>("StockManagementAPI");
        httpClient.BaseAddress = new Uri($"http://{apiHostAndPort}/api");
        _restClient = RestService.For<IStockManagementAPI>(
            httpClient,
            new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer()
            });
    }

    public async Task<List<Stock>> GetStocks()
    {
        return await _restClient.GetStocks();
    }
    public async Task<Stock> GetStockByTicker([AliasAs("id")] string ticker)
    {
        try
        {
            return await _restClient.GetStockByTicker(ticker);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                throw;
            }
        }
    }

    public async Task RegisterStock(RegisterStock command)
    {
        await _restClient.RegisterStock(command);
    }
}