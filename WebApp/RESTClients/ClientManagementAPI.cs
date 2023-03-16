namespace WebApp.RESTClients;

public class ClientManagementAPI : IClientManagementAPI
{
    private IClientManagementAPI _restClient;

    public ClientManagementAPI(IConfiguration config, HttpClient httpClient)
    {
        string apiHostAndPort = config.GetSection("APIServiceLocations").GetValue<string>("ClientManagementAPI");
        httpClient.BaseAddress = new Uri($"http://{apiHostAndPort}/api");
        _restClient = RestService.For<IClientManagementAPI>(
            httpClient,
            new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer()
            });
    }

    public async Task<List<Client>> GetClients()
    {
        return await _restClient.GetClients();
    }

    public async Task<Client> GetClientById([AliasAs("id")] string clientId)
    {
        try
        {
            return await _restClient.GetClientById(clientId);
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

    public async Task RegisterClient(RegisterClient command)
    {
        await _restClient.RegisterClient(command);
    }
}