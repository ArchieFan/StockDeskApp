namespace StockDesk.InvoiceService.Repositories;

public interface IInvoiceRepository
{
    Task RegisterClientAsync(Client client);
    Task<Client> GetClientAsync(string clientId);
    Task RegisterTradingAsync(Trading job);
    Task MarkTradingAsFinished(string jobId, DateTime startTime, DateTime endTime);
    Task<IEnumerable<Trading>> GetTradingsToBeInvoicedAsync();
    Task RegisterInvoiceAsync(Invoice invoice);
}