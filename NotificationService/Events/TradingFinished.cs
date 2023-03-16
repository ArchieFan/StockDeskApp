namespace StockDesk.NotificationService.Events;

public class TradingFinished : Event
{
    public readonly string JobId;

    public TradingFinished(Guid messageId, string jobId) :
        base(messageId)
    {
        JobId = jobId;
    }
}