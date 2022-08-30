namespace StockDesk.NotificationService.Model;

public class Trading
{
    public string JobId { get; set; }
    public string Ticker { get; set; }
    public string ClientId { get; set; }
    public DateTime StartTime { get; set; }
    public string Description { get; set; }
}