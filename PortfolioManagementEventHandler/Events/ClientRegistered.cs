namespace StockDesk.PortfolioManagementEventHandler.Events;

public class ClientRegistered : Event
{
    public readonly string ClientId;
    public readonly string Name;
    public readonly string TelephoneNumber;

    public ClientRegistered(Guid messageId, string clientId, string name, string telephoneNumber) :
        base(messageId)
    {
        ClientId = clientId;
        Name = name;
        TelephoneNumber = telephoneNumber;
    }
}