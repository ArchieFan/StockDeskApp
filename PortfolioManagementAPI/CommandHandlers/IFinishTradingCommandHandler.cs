namespace StockDesk.PortfolioManagementAPI.CommandHandlers;

public interface IFinishTradingCommandHandler
{
    Task<PortfolioPlanning> HandleCommandAsync(DateTime planningDate, FinishTrading command);
}