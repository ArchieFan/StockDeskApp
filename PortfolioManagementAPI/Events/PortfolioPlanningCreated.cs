namespace StockDesk.PortfolioManagementAPI.Events;

public class PortfolioPlanningCreated : Event
{
    public readonly DateTime Date;

    public PortfolioPlanningCreated(Guid messageId, DateTime date) : base(messageId)
    {
        Date = date;
    }
}