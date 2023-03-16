namespace StockDesk.PortfolioManagementAPI.Domain.BusinessRules;

public static class PortfolioPlanningRules
{
    /// <summary>
    /// The maximum number of parallel jobs in the portfolio (restricted by the available workstations).
    /// </summary>
    private const int AVAILABLE_WORKSTATIONS = 3;

    public static void NumberOfParallelTradingsMustNotExceedAvailableWorkStations(
        this PortfolioPlanning planning, PlanTrading command)
    {
        if (planning.Jobs.Count(j => j.PlannedTimeslot.OverlapsWith(command.StartTime, command.EndTime)) >= AVAILABLE_WORKSTATIONS)
        {
            throw new BusinessRuleViolationException($"Trading overlaps with more than {AVAILABLE_WORKSTATIONS} other jobs.");
        }
    }

    public static void NumberOfParallelTradingsOnAStockMustNotExceedOne(
        this PortfolioPlanning planning, PlanTrading command)
    {
        if (planning.Jobs.Any(j => j.Stock.Id == command.StockInfo.Ticker &&
                j.PlannedTimeslot.OverlapsWith(command.StartTime, command.EndTime)))
        {
            throw new BusinessRuleViolationException($"Only 1 trading job can be executed on a stock during a certain time-slot.");
        }
    }
}