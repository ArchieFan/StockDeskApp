namespace StockDesk.WebApp.Mappers;

public static class Mappers
{
    public static RegisterClient MapToRegisterClient(this ClientManagementNewViewModel source) => new RegisterClient
    (
        Guid.NewGuid(),
        Guid.NewGuid().ToString("N"),
        source.Client.Name,
        source.Client.Address,
        source.Client.PostalCode,
        source.Client.City,
        source.Client.TelephoneNumber,
        source.Client.EmailAddress
    );

    public static RegisterStock MapToRegisterStock(this StockManagementNewViewModel source) => new RegisterStock(
        Guid.NewGuid(),
        source.Stock.Ticker,
        source.Stock.CompanyName,
        source.Stock.Industry,
        source.Stock.MarketCap,
        source.SelectedClientId
    );
}