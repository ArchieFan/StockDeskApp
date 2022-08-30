namespace StockDesk.PortfolioManagementAPI.Domain.ValueObjects;

public class PortfolioPlanningId : ValueObject
{
    private const string DATE_FORMAT = "yyyy-MM-dd";
    public string Value { get; private set; }

    public static PortfolioPlanningId Create(DateTime date)
    {
        return new PortfolioPlanningId { Value = date.ToString(DATE_FORMAT) };
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public static implicit operator string(PortfolioPlanningId id) => id.Value;
    public static implicit operator DateTime(PortfolioPlanningId id) =>
        DateTime.ParseExact(id.Value, DATE_FORMAT, CultureInfo.InvariantCulture);
}