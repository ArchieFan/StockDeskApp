namespace StockDesk.NotificationService.Repositories;

public class SqlServerNotificationRepository : INotificationRepository
{
    private string _connectionString;

    public SqlServerNotificationRepository(string connectionString)
    {
        _connectionString = connectionString;

        // init db
        Log.Information("Initialize Database");

        Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(10, r => TimeSpan.FromSeconds(10), (ex, ts) => { Log.Error("Error connecting to DB. Retrying in 10 sec."); })
        .ExecuteAsync(InitializeDBAsync)
        .Wait();
    }

    public async Task<Client> GetClientAsync(string clientId)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            return await conn.QueryFirstOrDefaultAsync<Client>("select * from Client where ClientId = @ClientId",
                new { ClientId = clientId });
        }
    }

    public async Task RegisterTradingAsync(Trading job)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string sql =
                "insert into Trading(JobId, Ticker, ClientId, StartTime, Description) " +
                "values(@JobId, @Ticker, @ClientId, @StartTime, @Description);";
            await conn.ExecuteAsync(sql, job);
        }
    }

    public async Task RegisterClientAsync(Client client)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string sql =
                "insert into Client(ClientId, Name, TelephoneNumber, EmailAddress) " +
                "values(@ClientId, @Name, @TelephoneNumber, @EmailAddress);";
            await conn.ExecuteAsync(sql, client);
        }
    }

    public async Task<IEnumerable<Trading>> GetTradingsForTodayAsync(DateTime date)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            return await conn.QueryAsync<Trading>(
                "select * from Trading where StartTime >= @Today and StartTime < @Tomorrow",
                new { Today = date.Date, Tomorrow = date.AddDays(1).Date });
        }
    }

    public async Task RemoveTradingsAsync(IEnumerable<string> jobIds)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string sql =
                "delete Trading " +
                "where JobId = @JobId;";
            await conn.ExecuteAsync(sql, jobIds.Select(j => new { JobId = j }));
        }
    }

    private async Task InitializeDBAsync()
    {
        using (SqlConnection conn = new SqlConnection(_connectionString.Replace("Notification", "master")))
        {
            await conn.OpenAsync();

            // create database
            string sql =
                "IF NOT EXISTS(SELECT * FROM master.sys.databases WHERE name='Notification') CREATE DATABASE Notification;";

            await conn.ExecuteAsync(sql);
        }

        // create tables
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            // create tables
            string sql = "IF OBJECT_ID('Client') IS NULL " +
                  "CREATE TABLE Client (" +
                  "  ClientId varchar(50) NOT NULL," +
                  "  Name varchar(50) NOT NULL," +
                  "  TelephoneNumber varchar(50)," +
                  "  EmailAddress varchar(50)," +
                  "  PRIMARY KEY(ClientId));" +

                  "IF OBJECT_ID('Trading') IS NULL " +
                  "CREATE TABLE Trading (" +
                  "  JobId varchar(50) NOT NULL," +
                  "  Ticker varchar(50) NOT NULL," +
                  "  ClientId varchar(50) NOT NULL," +
                  "  StartTime datetime2 NOT NULL," +
                  "  Description varchar(250) NOT NULL," +
                  "  PRIMARY KEY(JobId));";

            await conn.ExecuteAsync(sql);
        }
    }
}