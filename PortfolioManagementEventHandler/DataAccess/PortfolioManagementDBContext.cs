namespace StockDesk.PortfolioManagementEventHandler.DataAccess;

public class PortfolioManagementDBContext : DbContext
{
    public PortfolioManagementDBContext()
    { }

    public PortfolioManagementDBContext(DbContextOptions<PortfolioManagementDBContext> options) : base(options)
    { }

    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Trading> Tradings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Stock>().HasKey(entity => entity.Ticker);
        builder.Entity<Stock>().ToTable("Stock");

        builder.Entity<Client>().HasKey(entity => entity.ClientId);
        builder.Entity<Client>().ToTable("Client");

        builder.Entity<Trading>().HasKey(entity => entity.Id);
        builder.Entity<Trading>().ToTable("Trading");

        base.OnModelCreating(builder);
    }
}