namespace WebApp.RESTClients;

public interface IPortfolioManagementAPI
{
    [Get("/portfolioplanning/{planningDate}")]
    Task<PortfolioPlanning> GetPortfolioPlanning(string planningDate);

    [Get("/portfolioplanning/{planningDate}/jobs/{jobId}")]
    Task<Trading> GetTrading(string planningDate, string jobId);

    [Post("/portfolioplanning/{planningDate}")]
    Task RegisterPlanning(string planningDate, RegisterPlanning cmd);

    [Post("/portfolioplanning/{planningDate}/jobs")]
    Task PlanTrading(string planningDate, PlanTrading cmd);

    [Put("/portfolioplanning/{planningDate}/jobs/{jobId}/finish")]
    Task FinishTrading(string planningDate, string jobId, FinishTrading cmd);

    [Get("/refdata/clients")]
    Task<List<Client>> GetClients();

    [Get("/refdata/clients/{id}")]
    Task<Client> GetClientById(string id);

    [Get("/refdata/stocks")]
    Task<List<Stock>> GetStocks();

    [Get("/refdata/stocks/{id}")]
    Task<Stock> GetStockByTicker([AliasAs("id")] string ticker);
}