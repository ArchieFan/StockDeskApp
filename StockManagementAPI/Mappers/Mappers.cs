namespace StockDesk.StockManagementAPI.Mappers;

public static class Mappers
{
    public static Stock MapToStock(this RegisterStock command) => new Stock
    {
        Ticker = command.Ticker,
        CompanyName = command.CompanyName,
        Industry = command.Industry,
        MarketCap = command.MarketCap,
        OwnerId = command.OwnerId
    };
}