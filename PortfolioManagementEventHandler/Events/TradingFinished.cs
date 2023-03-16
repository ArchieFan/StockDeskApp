namespace StockDesk.PortfolioManagementEventHandler.Events;

public class TradingFinished : Event
{
    public readonly Guid JobId;
    public readonly DateTime StartTime;
    public readonly DateTime EndTime;
    public readonly string Notes;

    public TradingFinished(Guid messageId, Guid jobId, DateTime startTime, DateTime endTime, string notes) :
        base(messageId)
    {
        JobId = jobId;
        StartTime = startTime;
        EndTime = endTime;
        Notes = notes;
    }
}