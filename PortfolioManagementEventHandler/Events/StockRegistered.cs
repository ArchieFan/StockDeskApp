﻿namespace StockDesk.PortfolioManagementEventHandler.Events;

public class StockRegistered : Event
{
    public readonly string Ticker;
    public readonly string CompanyName;
    public readonly string Industry;
    public readonly decimal MarketCap;
    public readonly string OwnerId;

    public StockRegistered(Guid messageId, string ticker, string companyname, string industry, decimal marketcap,  string ownerId) :
        base(messageId)
    {
        Ticker = ticker;
        CompanyName = companyname;
        Industry = industry;
        MarketCap = marketcap;
        OwnerId = ownerId;
    }
}