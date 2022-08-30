namespace StockDesk.WebApp.Commands;

public class PlanTrading : Command
{
    public readonly Guid JobId;
    public readonly DateTime StartTime;
    public readonly DateTime EndTime;
    public readonly (string Id, string Name, string TelephoneNumber) ClientInfo;
    public readonly (string Ticker, string CompanyName, string Industry, decimal MarketCap) StockInfo;
    public readonly string Description;

    public PlanTrading(Guid messageId, Guid jobId, DateTime startTime, DateTime endTime,
        (string Id, string Name, string TelephoneNumber) clientInfo,
        (string Ticker, string CompanyName, string Industry, decimal MarketCap) stockInfo,
        string description) : base(messageId)
    {
        JobId = jobId;
        StartTime = startTime;
        EndTime = endTime;
        ClientInfo = clientInfo;
        StockInfo = stockInfo;
        Description = description;
    }
}