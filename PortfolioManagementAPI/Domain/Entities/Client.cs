namespace StockDesk.PortfolioManagementAPI.Domain.Entities;

public class Client : Entity<string>
{
    public string Name { get; private set; }
    public string TelephoneNumber { get; private set; }

    public Client(string clientId, string name, string telephoneNumber) : base(clientId)
    {
        Name = name;
        TelephoneNumber = telephoneNumber;
    }
}