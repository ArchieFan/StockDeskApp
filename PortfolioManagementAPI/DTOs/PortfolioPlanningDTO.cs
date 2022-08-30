namespace StockDesk.PortfolioManagementAPI.DTOs;

public class PortfolioPlanningDTO
{
    public DateTime Date { get; set; }
    public List<TradingDTO> Jobs { get; set; }
}