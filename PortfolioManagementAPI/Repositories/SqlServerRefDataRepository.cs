namespace StockDesk.PortfolioManagementAPI.Repositories;

using StockDesk.PortfolioManagementAPI.Repositories.Model;

public class SqlServerRefDataRepository : IStockRepository, IClientRepository
{
    private string _connectionString;

    public SqlServerRefDataRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<Client>> GetClientsAsync()
    {
        List<Client> clients = new List<Client>();
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            try
            {
                var clientsSelection = await conn.QueryAsync<Client>("select * from Client");

                if (clientsSelection != null)
                {
                    clients.AddRange(clientsSelection);
                }
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex);
            }
        }

        return clients;
    }


    public async Task<IEnumerable<Model.Stock>> GetStocksAsync()
    {
        List<Stock> stocks = new List<Stock>();
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            try
            {
                var stockSelection = await conn.QueryAsync<Stock>("select * from Stock");

                if (stockSelection != null)
                {
                    stocks.AddRange(stockSelection);
                }
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex);
            }
        }

        return stocks;
    }

    public async Task<Stock> GetStockAsync(string ticker)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            try
            {
                return await conn.QueryFirstOrDefaultAsync<Stock>("select * from Stock where Ticker = @Ticker",
                    new { Ticker = ticker });

            }
            catch (SqlException ex)
            {
                HandleSqlException(ex);
            }
            return null;
        }
    }

    public async Task<Client> GetClientAsync(string clientId)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            try
            {
                return await conn.QueryFirstOrDefaultAsync<Client>("select * from Client where ClientId = @ClientId",
                    new { ClientId = clientId });
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex);
            }
            return null;
        }
    }


    private static void HandleSqlException(SqlException ex)
    {
        if (ex.Errors.Count > 0)
        {
            for (int i = 0; i < ex.Errors.Count; i++)
            {
                if (ex.Errors[i].Number == 4060)
                {
                    throw new DatabaseNotCreatedException("PortfolioManagement database not found. This database is automatically created by the PortfolioManagementEventHandler. Run this service first.");
                }
            }
        }

        // rethrow original exception without poluting the stacktrace
        ExceptionDispatchInfo.Capture(ex).Throw();
    }
}