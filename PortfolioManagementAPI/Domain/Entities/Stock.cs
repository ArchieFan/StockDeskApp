namespace StockDesk.PortfolioManagementAPI.Domain.Entities;

public class Stock : Entity<Ticker>
{
    public string CompanyName { get; private set; }
    public string Industry { get; private set; }

    public decimal MarketCap { get; private set; }
    public string OwnerId { get; private set; }

    public Stock(Ticker ticker, string companyname, string industry, decimal marketcap,  string ownerId) : base(ticker)
    {
        CompanyName = companyname;
        Industry = industry;
        MarketCap = marketcap;
        OwnerId = ownerId;
    }
}