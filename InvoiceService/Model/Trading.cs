namespace StockDesk.InvoiceService.Model;

public class Trading
{
    public string JobId { get; set; }
    public string Ticker { get; set; }
    public string ClientId { get; set; }
    public string Description { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool Finished { get; set; }
    public bool InvoiceSent { get; set; }
}