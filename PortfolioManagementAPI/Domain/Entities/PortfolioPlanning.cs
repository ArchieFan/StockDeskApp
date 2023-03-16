namespace StockDesk.PortfolioManagementAPI.Domain.Entities;

public class PortfolioPlanning : AggregateRoot<PortfolioPlanningId>
{
    /// <summary>
    /// The list of Trading-jobs for this day. 
    /// </summary>
    public List<Trading> Jobs { get; private set; }

    public PortfolioPlanning(DateTime date) : base(PortfolioPlanningId.Create(date)) { }

    public PortfolioPlanning(DateTime date, IEnumerable<Event> events) : base(PortfolioPlanningId.Create(date), events) { }

    /// <summary>
    /// Creates a new instance of a portfolio-planning for the specified date.
    /// </summary>
    /// <param name="date">The date to create the planning for.</param>
    public static PortfolioPlanning Create(DateTime date)
    {
        PortfolioPlanning planning = new PortfolioPlanning(date);
        PortfolioPlanningCreated e = new PortfolioPlanningCreated(Guid.NewGuid(), date);
        planning.RaiseEvent(e);
        return planning;
    }

    public void PlanTrading(PlanTrading command)
    {
        // check business rules
        command.PlannedTradingShouldFallWithinOneBusinessDay();
        this.NumberOfParallelTradingsMustNotExceedAvailableWorkStations(command);
        this.NumberOfParallelTradingsOnAStockMustNotExceedOne(command);

        // handle event
        TradingPlanned e = command.MapToTradingPlanned();
        RaiseEvent(e);
    }

    public void FinishTrading(FinishTrading command)
    {
        // find job
        Trading job = Jobs.FirstOrDefault(j => j.Id == command.JobId);
        if (job == null)
        {
            throw new TradingNotFoundException($"Trading job with id {command.JobId} found.");
        }

        // check business rules
        job.FinishedTradingCanNotBeFinished();

        // handle event
        TradingFinished e = command.MapToTradingFinished();
        RaiseEvent(e);
    }

    /// <summary>
    /// Handles an event and updates the aggregate version.
    /// </summary>
    /// <remarks>Caution: this handles is also called while replaying events to restore state.
    /// So, do not execute any checks that could fail or introduce any side-effects in this handler.</remarks>
    protected override void When(dynamic @event)
    {
        Handle(@event);
    }

    private void Handle(PortfolioPlanningCreated e)
    {
        Jobs = new List<Trading>();
    }

    private void Handle(TradingPlanned e)
    {
        Trading job = new Trading(e.JobId);
        Client client = new Client(e.ClientInfo.Id, e.ClientInfo.Name, e.ClientInfo.TelephoneNumber);
        Ticker ticker = Ticker.Create(e.StockInfo.Ticker);
        Stock stock = new Stock(ticker, e.StockInfo.CompanyName, e.StockInfo.Industry, e.StockInfo.MarketCap, client.Id);
        Timeslot plannedTimeslot = Timeslot.Create(e.StartTime, e.EndTime);
        job.Plan(plannedTimeslot, stock, client, e.Description);
        Jobs.Add(job);
    }

    private void Handle(TradingFinished e)
    {
        Trading job = Jobs.FirstOrDefault(j => j.Id == e.JobId);
        Timeslot actualTimeslot = Timeslot.Create(e.StartTime, e.EndTime);
        job.Finish(actualTimeslot, e.Notes);
    }
}