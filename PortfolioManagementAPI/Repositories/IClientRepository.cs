namespace StockDesk.PortfolioManagementAPI.Repositories;

using StockDesk.PortfolioManagementAPI.Repositories.Model;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetClientsAsync();
    Task<Client> GetClientAsync(string clientId);
}