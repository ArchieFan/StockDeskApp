namespace StockDesk.WebApp.ViewModels;

public class PortfolioManagementNewViewModel
{
    public Guid Id { get; set; }

    public DateTime Date { get; set; }

    [Required]
    [DataType(DataType.Time)]
    [Display(Name = "Start Time")]
    public DateTime StartTime { get; set; }

    [Required]
    [Display(Name = "End Time")]
    [DataType(DataType.Time)]
    public DateTime EndTime { get; set; }

    [Required]
    [Display(Name = "Description")]
    public string Description { get; set; }

    public IEnumerable<SelectListItem> Stocks { get; set; }

    [Required(ErrorMessage = "Stock is required")]
    [Display(Name = "Stock")]
    public string SelectedStockTicker { get; set; }

    public string Error { get; set; }
}