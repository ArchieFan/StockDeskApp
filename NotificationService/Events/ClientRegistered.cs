namespace StockDesk.NotificationService.Events;

public class ClientRegistered : Event
{
    public readonly string ClientId;
    public readonly string Name;
    public readonly string TelephoneNumber;
    public readonly string EmailAddress;

    public ClientRegistered(Guid messageId, string clientId, string name, string telephoneNumber, string emailAddress) :
        base(messageId)
    {
        ClientId = clientId;
        Name = name;
        TelephoneNumber = telephoneNumber;
        EmailAddress = emailAddress;
    }
}