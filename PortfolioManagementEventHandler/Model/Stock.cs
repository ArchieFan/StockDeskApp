namespace StockDesk.PortfolioManagementEventHandler.Model;

public class Stock
{
    public string Ticker { get; set; }
    public string CompanyName { get; set; }
    public string Industry { get; set; }
    public decimal MarketCap { get; set; }
    public string OwnerId { get; set; }
}