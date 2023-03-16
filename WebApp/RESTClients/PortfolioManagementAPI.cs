namespace WebApp.RESTClients;

public class PortfolioManagementAPI : IPortfolioManagementAPI
{
    private IPortfolioManagementAPI _restClient;

    public PortfolioManagementAPI(IConfiguration config, HttpClient httpClient)
    {
        string apiHostAndPort = config.GetSection("APIServiceLocations").GetValue<string>("PortfolioManagementAPI");
        httpClient.BaseAddress = new Uri($"http://{apiHostAndPort}/api");
        _restClient = RestService.For<IPortfolioManagementAPI>(
            httpClient,
            new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer()
            });
    }

    public async Task<PortfolioPlanning> GetPortfolioPlanning(string planningDate)
    {
        try
        {
            return await _restClient.GetPortfolioPlanning(planningDate);
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

    public async Task<Trading> GetTrading(string planningDate, string jobId)
    {
        try
        {
            return await _restClient.GetTrading(planningDate, jobId);
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

    public async Task RegisterPlanning(string planningDate, RegisterPlanning cmd)
    {
        await _restClient.RegisterPlanning(planningDate, cmd);
    }

    public async Task PlanTrading(string planningDate, PlanTrading cmd)
    {
        await _restClient.PlanTrading(planningDate, cmd);
    }

    public async Task FinishTrading(string planningDate, string jobId, FinishTrading cmd)
    {
        await _restClient.FinishTrading(planningDate, jobId, cmd);
    }

    public async Task<List<Client>> GetClients()
    {
        return await _restClient.GetClients();
    }

    public async Task<Client> GetClientById(string id)
    {
        try
        {
            return await _restClient.GetClientById(id);
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
}