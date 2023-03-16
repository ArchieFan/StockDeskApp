namespace StockDesk.NotificationService;

public class NotificationWorker : IHostedService, IMessageHandlerCallback
{
    IMessageHandler _messageHandler;
    INotificationRepository _repo;
    IEmailNotifier _emailNotifier;

    public NotificationWorker(IMessageHandler messageHandler, INotificationRepository repo, IEmailNotifier emailNotifier)
    {
        _messageHandler = messageHandler;
        _repo = repo;
        _emailNotifier = emailNotifier;
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
        try
        {
            JObject messageObject = MessageSerializer.Deserialize(message);
            switch (messageType)
            {
                case "ClientRegistered":
                    await HandleAsync(messageObject.ToObject<ClientRegistered>());
                    break;
                case "TradingPlanned":
                    await HandleAsync(messageObject.ToObject<TradingPlanned>());
                    break;
                case "TradingFinished":
                    await HandleAsync(messageObject.ToObject<TradingFinished>());
                    break;
                case "DayHasPassed":
                    await HandleAsync(messageObject.ToObject<DayHasPassed>());
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error while handling {messageType} event.");
        }

        return true;
    }

    private async Task HandleAsync(ClientRegistered cr)
    {
        Client client = new Client
        {
            ClientId = cr.ClientId,
            Name = cr.Name,
            TelephoneNumber = cr.TelephoneNumber,
            EmailAddress = cr.EmailAddress
        };

        Log.Information("Register client: {Id}, {Name}, {TelephoneNumber}, {Email}",
            client.ClientId, client.Name, client.TelephoneNumber, client.EmailAddress);

        await _repo.RegisterClientAsync(client);
    }

    private async Task HandleAsync(TradingPlanned mjp)
    {
        Trading job = new Trading
        {
            JobId = mjp.JobId.ToString(),
            ClientId = mjp.ClientInfo.Id,
            Ticker = mjp.StockInfo.Ticker,
            StartTime = mjp.StartTime,
            Description = mjp.Description
        };

        Log.Information("Register Trading Job: {Id}, {ClientId}, {StockTicker}, {StartTime}, {Description}",
            job.JobId, job.ClientId, job.Ticker, job.StartTime, job.Description);

        await _repo.RegisterTradingAsync(job);
    }

    private async Task HandleAsync(TradingFinished mjf)
    {
        Log.Information("Remove finished Trading Job: {Id}", mjf.JobId);

        await _repo.RemoveTradingsAsync(new string[] { mjf.JobId.ToString() });
    }

    private async Task HandleAsync(DayHasPassed dhp)
    {
        DateTime today = DateTime.Now;

        IEnumerable<Trading> jobsToNotify = await _repo.GetTradingsForTodayAsync(today);
        foreach (var jobsPerClient in jobsToNotify.GroupBy(job => job.ClientId))
        {
            // build notification body
            string clientId = jobsPerClient.Key;
            Client client = await _repo.GetClientAsync(clientId);
            StringBuilder body = new StringBuilder();
            body.AppendLine($"Dear {client.Name},\n");
            body.AppendLine($"We would like to remind you that you have appointed us to trade your stock(s):\n");
            foreach (Trading job in jobsPerClient)
            {
                body.AppendLine($"- {job.StartTime.ToString("dd-MM-yyyy")} at {job.StartTime.ToString("HH:mm")} : " +
                    $"{job.Description} on stock with ticker {job.Ticker}");
            }

            body.AppendLine($"Greetings,\n");
            body.AppendLine($"StockDesk Helpdesk");

            Log.Information("Sent notification to: {ClientName}", client.Name);

            // send notification
            await _emailNotifier.SendEmailAsync(
                client.EmailAddress, "noreply@StockDesk.com", "Stock WatchList Reminder", body.ToString());

            // remove jobs for which a notification was sent
            await _repo.RemoveTradingsAsync(jobsPerClient.Select(job => job.JobId));
        }
    }
}