namespace StockDesk.PortfolioManagementAPI.CommandHandlers;

public static class CommandHandlersDIRegistration
{
    public static void AddCommandHandlers(this IServiceCollection services)
    {
        services.AddTransient<IPlanTradingCommandHandler, PlanTradingCommandHandler>();
        services.AddTransient<IFinishTradingCommandHandler, FinishTradingCommandHandler>();
    }
}