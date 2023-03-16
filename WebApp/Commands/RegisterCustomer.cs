namespace StockDesk.WebApp.Commands;

public class RegisterClient : Command
{
    public readonly string ClientId;
    public readonly string Name;
    public readonly string Address;
    public readonly string PostalCode;
    public readonly string City;
    public readonly string TelephoneNumber;
    public readonly string EmailAddress;

    public RegisterClient(Guid messageId, string clientId, string name, string address, string postalCode, string city,
        string telephoneNumber, string emailAddress) : base(messageId)
    {
        ClientId = clientId;
        Name = name;
        Address = address;
        PostalCode = postalCode;
        City = city;
        TelephoneNumber = telephoneNumber;
        EmailAddress = emailAddress;
    }
}