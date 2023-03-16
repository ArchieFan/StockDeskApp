namespace StockDesk.WebApp.ViewModels;

public class StockManagementNewViewModel
{
    public Stock Stock { get; set; }
    public IEnumerable<SelectListItem> Clients { get; set; }
    [Required(ErrorMessage = "Owner is required")]
    public string SelectedClientId { get; set; }
}