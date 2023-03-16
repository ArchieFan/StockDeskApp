namespace StockDesk.PortfolioManagementAPI.CommandHandlers;

public class FinishTradingCommandHandler : IFinishTradingCommandHandler
{
    IMessagePublisher _messagePublisher;
    IEventSourceRepository<PortfolioPlanning> _planningRepo;

    public FinishTradingCommandHandler(IMessagePublisher messagePublisher, IEventSourceRepository<PortfolioPlanning> planningRepo)
    {
        _messagePublisher = messagePublisher;
        _planningRepo = planningRepo;
    }

    public async Task<PortfolioPlanning> HandleCommandAsync(DateTime planningDate, FinishTrading command)
    {
        // get planning
        var aggregateId = PortfolioPlanningId.Create(planningDate);
        var planning = await _planningRepo.GetByIdAsync(aggregateId);
        if (planning == null)
        {
            return null;
        }

        // handle command
        planning.FinishTrading(command);

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