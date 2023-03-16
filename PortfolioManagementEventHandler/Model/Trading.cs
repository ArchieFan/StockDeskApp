namespace StockDesk.PortfolioManagementEventHandler.Model;

public class Trading
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Stock Stock { get; set; }
    public Client Client { get; set; }
    public string Description { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public string Notes { get; set; }
    public DateTime PortfolioPlanningDate { get; set; }
}