namespace StockDesk.PortfolioManagementAPI.Mappers;

public static class Mappers
{
    public static TradingPlanned MapToTradingPlanned(this PlanTrading source) => new TradingPlanned(
        Guid.NewGuid(),
        source.JobId,
        source.StartTime,
        source.EndTime,
        source.ClientInfo,
        source.StockInfo,
        source.Description
    );

    public static TradingFinished MapToTradingFinished(this FinishTrading source) => new TradingFinished
    (
        Guid.NewGuid(),
        source.JobId,
        source.StartTime,
        source.EndTime,
        source.Notes
    );

    public static PortfolioPlanningDTO MapToDTO(this PortfolioPlanning source) =>
        new PortfolioPlanningDTO
        {
            Date = (DateTime)source.Id,
            Jobs = source.Jobs.Select(j => j.MapToDTO()).ToList(),
        };

    public static TradingDTO MapToDTO(this Trading source) =>
        new TradingDTO
        {
            Id = source.Id,
            StartTime = source.PlannedTimeslot.StartTime,
            EndTime = source.PlannedTimeslot.EndTime,
            Stock = source.Stock.MapToDTO(),
            Client = source.Client.MapToDTO(),
            Description = source.Description,
            ActualStartTime = source?.ActualTimeslot?.StartTime,
            ActualEndTime = source?.ActualTimeslot?.EndTime,
            Notes = source.Notes
        };

    public static StockDTO MapToDTO(this Stock source) =>
        new StockDTO
        {
            Ticker = source.Id.Value,
            CompanyName = source.CompanyName,
            Industry = source.Industry,
            MarketCap = source.MarketCap
        };

    public static ClientDTO MapToDTO(this Client source) =>
        new ClientDTO
        {
            ClientId = source.Id,
            Name = source.Name,
            TelephoneNumber = source.TelephoneNumber
        };
}