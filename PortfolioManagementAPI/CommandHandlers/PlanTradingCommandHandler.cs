namespace StockDesk.PortfolioManagementAPI.CommandHandlers;

public class PlanTradingCommandHandler : IPlanTradingCommandHandler
{
    IMessagePublisher _messagePublisher;
    IEventSourceRepository<PortfolioPlanning> _planningRepo;

    public PlanTradingCommandHandler(IMessagePublisher messagePublisher, IEventSourceRepository<PortfolioPlanning> planningRepo)
    {
        _messagePublisher = messagePublisher;
        _planningRepo = planningRepo;
    }

    public async Task<PortfolioPlanning> HandleCommandAsync(DateTime planningDate, PlanTrading command)
    {
        // get or create portfolio-planning
        var aggregateId = PortfolioPlanningId.Create(planningDate);
        var planning = await _planningRepo.GetByIdAsync(aggregateId);
        if (planning == null)
        {
            planning = PortfolioPlanning.Create(planningDate);
        }

        // handle command
        planning.PlanTrading(command);

        // persist
        IEnumerable<Event> events = planning.GetEvents();
        await _planningRepo.SaveAsync(
            planning.Id, planning.OriginalVersion, planning.Version, events);

        // publish event
        foreach (var e in events)
        {
            await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");
        }

        // return result
        return planning;
    }
}