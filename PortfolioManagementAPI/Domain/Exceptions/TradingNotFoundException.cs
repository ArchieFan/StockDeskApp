namespace StockDesk.PortfolioManagementAPI.Domain.Exceptions;

public class TradingNotFoundException : Exception
{
    public TradingNotFoundException()
    {
    }

    public TradingNotFoundException(string message) : base(message)
    {
    }

    public TradingNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}