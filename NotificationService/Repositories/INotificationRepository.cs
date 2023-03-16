namespace StockDesk.NotificationService.Repositories;

public interface INotificationRepository
{
    Task RegisterClientAsync(Client client);
    Task RegisterTradingAsync(Trading job);
    Task<IEnumerable<Trading>> GetTradingsForTodayAsync(DateTime date);
    Task<Client> GetClientAsync(string clientId);
    Task RemoveTradingsAsync(IEnumerable<string> jobIds);
}