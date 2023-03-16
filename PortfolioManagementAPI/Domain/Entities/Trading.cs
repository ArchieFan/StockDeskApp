namespace StockDesk.PortfolioManagementAPI.Domain.Entities;

public class Trading : Entity<Guid>
{
    public Timeslot PlannedTimeslot { get; private set; }
    public Stock Stock { get; private set; }
    public Client Client { get; private set; }
    public string Description { get; private set; }
    public Timeslot ActualTimeslot { get; private set; }
    public string Notes { get; private set; }
    public string Status => (ActualTimeslot == null) ? "Planned" : "Completed";

    public Trading(Guid id) : base(id)
    {

    }

    public void Plan(Timeslot timeslot, Stock stock, Client client, string description)
    {
        PlannedTimeslot = timeslot;
        Stock = stock;
        Client = client;
        Description = description;
    }

    public void Finish(Timeslot actualTimeslot, string notes)
    {

        ActualTimeslot = actualTimeslot;
        Notes = notes;
    }

}