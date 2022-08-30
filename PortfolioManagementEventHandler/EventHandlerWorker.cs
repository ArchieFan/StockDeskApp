namespace StockDesk.PortfolioManagementEventHandler;

public class EventHandlerWorker : IHostedService, IMessageHandlerCallback
{
    PortfolioManagementDBContext _dbContext;
    IMessageHandler _messageHandler;

    public EventHandlerWorker(IMessageHandler messageHandler, PortfolioManagementDBContext dbContext)
    {
        _messageHandler = messageHandler;
        _dbContext = dbContext;
    }

    public void Start()
    {
        _messageHandler.Start(this);
    }

    public void Stop()
    {
        _messageHandler.Stop();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _messageHandler.Start(this);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _messageHandler.Stop();
        return Task.CompletedTask;
    }

    public async Task<bool> HandleMessageAsync(string messageType, string message)
    {
        JObject messageObject = MessageSerializer.Deserialize(message);
        try
        {
            switch (messageType)
            {
                case "ClientRegistered":
                    await HandleAsync(messageObject.ToObject<ClientRegistered>());
                    break;
                case "StockRegistered":
                    await HandleAsync(messageObject.ToObject<StockRegistered>());
                    break;
                case "TradingPlanned":
                    await HandleAsync(messageObject.ToObject<TradingPlanned>());
                    break;
                case "TradingFinished":
                    await HandleAsync(messageObject.ToObject<TradingFinished>());
                    break;
            }
        }
        catch (Exception ex)
        {
            string messageId = messageObject.Property("MessageId") != null ? messageObject.Property("MessageId").Value<string>() : "[unknown]";
            Log.Error(ex, "Error while handling {MessageType} message with id {MessageId}.", messageType, messageId);
        }

        // always akcnowledge message - any errors need to be dealt with locally.
        return true;
    }

    private async Task<bool> HandleAsync(StockRegistered e)
    {
        Log.Information("Register Stock: {Ticker}, {CompanyName}, {Industry}, {MarketCap}, Owner Id: {OwnerId}",
            e.Ticker, e.CompanyName, e.Industry, e.MarketCap, e.OwnerId);

        try
        {
            await _dbContext.Stocks.AddAsync(new Stock
            {
                Ticker = e.Ticker,
                CompanyName = e.CompanyName,
                Industry = e.Industry,
                MarketCap = e.MarketCap,
                OwnerId = e.OwnerId
            });
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            Console.WriteLine($"Skipped adding stock with license number {e.Ticker}.");
        }

        return true;
    }

    private async Task<bool> HandleAsync(ClientRegistered e)
    {
        Log.Information("Register Client: {ClientId}, {Name}, {TelephoneNumber}",
            e.ClientId, e.Name, e.TelephoneNumber);

        try
        {
            await _dbContext.Clients.AddAsync(new Client
            {
                ClientId = e.ClientId,
                Name = e.Name,
                TelephoneNumber = e.TelephoneNumber
            });
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            Log.Warning("Skipped adding client with client id {ClientId}.", e.ClientId);
        }

        return true;
    }

    private async Task<bool> HandleAsync(TradingPlanned e)
    {
        Log.Information("Register trading Job: {JobId}, {StartTime}, {EndTime}, {ClientName}, {Ticker}",
            e.JobId, e.StartTime, e.EndTime, e.ClientInfo.Name, e.StockInfo.Ticker);

        try
        {
            // determine client
            Client client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.ClientId == e.ClientInfo.Id);
            if (client == null)
            {
                client = new Client
                {
                    ClientId = e.ClientInfo.Id,
                    Name = e.ClientInfo.Name,
                    TelephoneNumber = e.ClientInfo.TelephoneNumber
                };
            }

            // determine stock
            Stock stock = await _dbContext.Stocks.FirstOrDefaultAsync(v => v.Ticker == e.StockInfo.Ticker);
            if (stock == null)
            {
                stock = new Stock
                {
                    Ticker = e.StockInfo.Ticker,
                    CompanyName = e.StockInfo.CompanyName,
                    Industry = e.StockInfo.Industry,
                    MarketCap = e.StockInfo.MarketCap,
                    OwnerId = client.ClientId
                };
            }

            // insert maintetancejob
            await _dbContext.Tradings.AddAsync(new Trading
            {
                Id = e.JobId,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Client = client,
                Stock = stock,
                PortfolioPlanningDate = e.StartTime.Date,
                Description = e.Description
            });
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            Log.Warning("Skipped adding trading job with id {JobId}.", e.JobId);
        }

        return true;
    }

    private async Task<bool> HandleAsync(TradingFinished e)
    {
        Log.Information("Finish trading job: {JobId}, {ActualStartTime}, {EndTime}",
            e.JobId, e.StartTime, e.EndTime);

        try
        {
            // insert maintetancejob
            var job = await _dbContext.Tradings.FirstOrDefaultAsync(j => j.Id == e.JobId);
            job.ActualStartTime = e.StartTime;
            job.ActualEndTime = e.EndTime;
            job.Notes = e.Notes;
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            Log.Warning("Skipped adding trading job with id {JobId}.", e.JobId);
        }

        return true;
    }
}