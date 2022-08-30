namespace StockDesk.PortfolioManagementAPI.Repositories;

using StockDesk.PortfolioManagementAPI.Repositories.Model;

public interface IStockRepository
{
    Task<IEnumerable<Stock>> GetStocksAsync();
    Task<Stock> GetStockAsync(string ticker);
}