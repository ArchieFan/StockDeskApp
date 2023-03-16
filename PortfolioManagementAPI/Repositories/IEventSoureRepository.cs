namespace StockDesk.PortfolioManagementAPI.Repositories;

public interface IEventSourceRepository<T>
{
    Task<T> GetByIdAsync(string id);
    Task SaveAsync(string id, int originalVersion, int newVersion, IEnumerable<Event> newEvents);
}