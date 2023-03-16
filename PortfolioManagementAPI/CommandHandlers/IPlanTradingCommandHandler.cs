namespace StockDesk.PortfolioManagementAPI.CommandHandlers;

public interface IPlanTradingCommandHandler
{
    Task<PortfolioPlanning> HandleCommandAsync(DateTime planningDate, PlanTrading command);
}