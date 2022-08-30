namespace StockDesk.InvoiceService.Events;

public class TradingFinished : Event
{
    public readonly string JobId;
    public readonly DateTime StartTime;
    public readonly DateTime EndTime;

    public TradingFinished(Guid messageId, string jobId, DateTime startTime, DateTime endTime) :
        base(messageId)
    {
        JobId = jobId;
        StartTime = startTime;
        EndTime = endTime;
    }
}