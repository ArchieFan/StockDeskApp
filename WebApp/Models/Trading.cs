namespace StockDesk.WebApp.Models
{
    public class Trading
    {
        public Guid Id { get; set; }

        public string Status
        {
            get
            {
                return ActualEndTime != null ? "Completed" : "Planned";
            }
        }

        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        [Display(Name = "Stock")]
        public Stock Stock { get; set; }

        [Display(Name = "Client")]
        public Client Client { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Started at")]
        public DateTime? ActualStartTime { get; set; }

        [Display(Name = "Completed at")]
        public DateTime? ActualEndTime { get; set; }

        [Display(Name = "Fund manager notes")]
        public string Notes { get; set; }

        [Display(Name = "Date")]
        public DateTime PortfolioPlanningDate { get; set; }
    }
}
