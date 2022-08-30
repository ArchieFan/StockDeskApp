namespace StockDesk.PortfolioManagementAPI.Commands;

public class FinishTrading : Command
{
    public readonly Guid JobId;
    public readonly DateTime StartTime;
    public readonly DateTime EndTime;
    public readonly string Notes;

    public FinishTrading(Guid messageId, Guid jobId, DateTime startTime, DateTime endTime, string notes) :
        base(messageId)
    {
        JobId = jobId;
        StartTime = startTime;
        EndTime = endTime;
        Notes = notes;
    }
}