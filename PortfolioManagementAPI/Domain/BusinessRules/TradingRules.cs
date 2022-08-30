namespace StockDesk.PortfolioManagementAPI.Domain.BusinessRules;

public static class TradingRules
{
    public static void PlannedTradingShouldFallWithinOneBusinessDay(this PlanTrading command)
    {
        if (!Timeslot.Create(command.StartTime, command.EndTime).IsWithinOneDay())
        {
            throw new BusinessRuleViolationException("Start-time and end-time of a trading Job must be within a 1 day.");
        }
    }

    public static void FinishedTradingCanNotBeFinished(this Trading job)
    {
        if (job.Status == "Completed")
        {
            throw new BusinessRuleViolationException($"An already finished job can not be finished.");
        }
    }
}