namespace WebApp.RESTClients;

public interface IStockManagementAPI
{
    [Get("/stocks")]
    Task<List<Stock>> GetStocks();

    [Get("/stocks/{id}")]
    Task<Stock> GetStockByTicker([AliasAs("id")] string ticker);

    [Post("/stocks")]
    Task RegisterStock(RegisterStock command);
}