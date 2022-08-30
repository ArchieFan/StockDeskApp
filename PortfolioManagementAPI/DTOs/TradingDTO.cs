namespace StockDesk.PortfolioManagementAPI.DTOs;

public class TradingDTO
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public StockDTO Stock { get; set; }
    public ClientDTO Client { get; set; }
    public string Description { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public string Notes { get; set; }
    public string Status => (!ActualStartTime.HasValue && !ActualEndTime.HasValue) ? "Planned" : "Completed";
}