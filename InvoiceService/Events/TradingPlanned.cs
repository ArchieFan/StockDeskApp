namespace StockDesk.InvoiceService.Events;

public class TradingPlanned : Event
{
    public readonly string JobId;
    public readonly (string Id, string Name, string TelephoneNumber) ClientInfo;
    public readonly (string Ticker, string CompanyName, string Type) StockInfo;
    public readonly string Description;

    public TradingPlanned(Guid messageId, string jobId, (string Id, string Name, string TelephoneNumber) clientInfo,
        (string Ticker, string CompanyName, string Type) stockInfo, string description) : base(messageId)
    {
        JobId = jobId;
        ClientInfo = clientInfo;
        StockInfo = stockInfo;
        Description = description;
    }
}