namespace WebApp.RESTClients;

public interface IClientManagementAPI
{
    [Get("/clients")]
    Task<List<Client>> GetClients();

    [Get("/clients/{id}")]
    Task<Client> GetClientById([AliasAs("id")] string clientId);

    [Post("/clients")]
    Task RegisterClient(RegisterClient command);
}