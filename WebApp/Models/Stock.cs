namespace StockDesk.WebApp.Models;

public class Stock
{
    [Required]
    [Display(Name = "Ticker Symbol")]
    //[RegularExpression(@"^((\d{1,3}|[a-zA-Z]{1,3})-){2}(\d{1,3}|[a-zA-Z]{1,3})$", ErrorMessage = "Ticker is not in a valid format.")]
    public string Ticker { get; set; }

    [Required]
    [Display(Name = "Company Name")]
    public string CompanyName { get; set; }

    [Required]
    [Display(Name = "Industry")]
    public string Industry { get; set; }

    [Required]
    [Display(Name = "Market Cap")]
    public decimal MarketCap { get; set; }

    [Display(Name = "Owner")]
    public string OwnerId { get; set; }
}