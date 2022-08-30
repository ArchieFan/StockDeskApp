namespace StockDesk.StockManagement.DataAccess;

public class StockManagementDBContext : DbContext
{
    public StockManagementDBContext(DbContextOptions<StockManagementDBContext> options) : base(options)
    {
    }

    public DbSet<Stock> Stocks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Stock>().HasKey(m => m.Ticker);
        builder.Entity<Stock>().ToTable("Stock");
        base.OnModelCreating(builder);
    }

    public void MigrateDB()
    {
        Policy
            .Handle<Exception>()
            .WaitAndRetry(10, r => TimeSpan.FromSeconds(10))
            .Execute(() => Database.Migrate());
    }
}