namespace StockDesk.ClientManagementAPI.DataAccess;

public class ClientManagementDBContext : DbContext
{
    public ClientManagementDBContext(DbContextOptions<ClientManagementDBContext> options) : base(options)
    {

    }

    public DbSet<Client> Clients { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Client>().HasKey(m => m.ClientId);
        builder.Entity<Client>().ToTable("Client");
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