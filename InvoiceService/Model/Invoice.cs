namespace StockDesk.InvoiceService.Model;

public class Invoice
{
    public string InvoiceId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string ClientId { get; set; }
    public decimal Amount { get; set; }
    public string Specification { get; set; }
    public string JobIds { get; set; }
}
