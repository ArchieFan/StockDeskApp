namespace StockDesk.InvoiceService.Events;

public class ClientRegistered : Event
{
    public readonly string ClientId;
    public readonly string Name;
    public readonly string Address;
    public readonly string PostalCode;
    public readonly string City;


    public ClientRegistered(Guid messageId, string clientId, string name, string address, string postalCode, string city) : base(messageId)
    {
        ClientId = clientId;
        Name = name;
        Address = address;
        PostalCode = postalCode;
        City = city;
    }
}
