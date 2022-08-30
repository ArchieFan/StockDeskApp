namespace StockDesk.InvoiceService.Repositories
{
    public class SqlServerInvoiceRepository : IInvoiceRepository
    {
        private string _connectionString;

        public SqlServerInvoiceRepository(string connectionString)
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
                    "insert into Trading(JobId, Ticker, ClientId, Description, Finished, InvoiceSent) " +
                    "values(@JobId, @Ticker, @ClientId, @Description, 0, 0);";
                await conn.ExecuteAsync(sql, job);
            }
        }

        public async Task RegisterClientAsync(Client client)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql =
                    "insert into Client(ClientId, Name, Address, PostalCode, City) " +
                    "values(@ClientId, @Name, @Address, @PostalCode, @City);";
                await conn.ExecuteAsync(sql, client);
            }
        }

        public async Task MarkTradingAsFinished(string jobId, DateTime startTime, DateTime endTime)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query =
                    "update Trading " +
                    "set StartTime = @StartTime, " +
                    "    EndTime = @EndTime, " +
                    "    Finished = 1 " +
                    "where JobId = @JobId";
                await conn.ExecuteAsync(query, new { JobId = jobId, StartTime = startTime, EndTime = endTime });
            }
        }

        public async Task<IEnumerable<Trading>> GetTradingsToBeInvoicedAsync()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query =
                    "select * from Trading " +
                    "where Finished = 1 " +
                    "and InvoiceSent = 0";
                return await conn.QueryAsync<Trading>(query);
            }
        }

        public async Task RegisterInvoiceAsync(Invoice invoice)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // persist invoice
                string sql =
                    "insert into Invoice(InvoiceId, InvoiceDate, ClientId, Amount, Specification, JobIds) " +
                    "values(@InvoiceId, @InvoiceDate, @ClientId, @Amount, @Specification, @JobIds);";
                await conn.ExecuteAsync(sql, invoice);

                // update jobs to indicate invoice sent
                var jobIds = invoice.JobIds.Split('|').Select(jobId => new { JobId = jobId });
                sql =
                    "update Trading " +
                    "set InvoiceSent = 1 " +
                    "where JobId = @JobId ";
                await conn.ExecuteAsync(sql, jobIds);
            }
        }

        private async Task InitializeDBAsync()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString.Replace("Invoicing", "master")))
            {
                await conn.OpenAsync();

                // create database
                string sql =
                    "IF NOT EXISTS(SELECT * FROM master.sys.databases WHERE name='Invoicing') CREATE DATABASE Invoicing;";

                await conn.ExecuteAsync(sql);
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // create tables
                string sql = "IF OBJECT_ID('Client') IS NULL " +
                      "CREATE TABLE Client (" +
                      "  ClientId varchar(50) NOT NULL," +
                      "  Name varchar(50) NOT NULL," +
                      "  Address varchar(50)," +
                      "  PostalCode varchar(50)," +
                      "  City varchar(50)," +
                      "  PRIMARY KEY(ClientId));" +

                      "IF OBJECT_ID('Trading') IS NULL " +
                      "CREATE TABLE Trading (" +
                      "  JobId varchar(50) NOT NULL," +
                      "  Ticker varchar(50) NOT NULL," +
                      "  ClientId varchar(50) NOT NULL," +
                      "  Description varchar(250) NOT NULL," +
                      "  StartTime datetime2 NULL," +
                      "  EndTime datetime2 NULL," +
                      "  Finished bit NOT NULL," +
                      "  InvoiceSent bit NOT NULL," +
                      "  PRIMARY KEY(JobId));" +

                      "IF OBJECT_ID('Invoice') IS NULL " +
                      "CREATE TABLE Invoice (" +
                      "  InvoiceId varchar(50) NOT NULL," +
                      "  InvoiceDate datetime2 NOT NULL," +
                      "  ClientId varchar(50) NOT NULL," +
                      "  Amount decimal(5,2) NOT NULL," +
                      "  Specification text," +
                      "  JobIds varchar(250)," +
                      "  PRIMARY KEY(InvoiceId));";

                await conn.ExecuteAsync(sql);
            }
        }        
    }
}
