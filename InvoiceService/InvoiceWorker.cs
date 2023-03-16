namespace StockDesk.InvoiceService;

public class InvoiceWorker : IHostedService, IMessageHandlerCallback
{
    private const decimal HOURLY_RATE = 1.50M;
    private IMessageHandler _messageHandler;
    private IInvoiceRepository _repo;
    private IEmailCommunicator _emailCommunicator;

    public InvoiceWorker(IMessageHandler messageHandler, IInvoiceRepository repo, IEmailCommunicator emailCommunicator)
    {
        _messageHandler = messageHandler;
        _repo = repo;
        _emailCommunicator = emailCommunicator;
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
        Log.Information("Register client: {Id}, {Name}, {Address}, {PostalCode}, {City}",
            cr.ClientId, cr.Name, cr.Address, cr.PostalCode, cr.City);

        Client client = new Client
        {
            ClientId = cr.ClientId,
            Name = cr.Name,
            Address = cr.Address,
            PostalCode = cr.PostalCode,
            City = cr.City
        };

        await _repo.RegisterClientAsync(client);
    }

    private async Task HandleAsync(TradingPlanned mjp)
    {
        Log.Information("Register Trading Job: {Id}, {Description}, {ClientId}, {StockTicker}",
            mjp.JobId, mjp.Description, mjp.ClientInfo.Id, mjp.StockInfo.Ticker);

        Trading job = new Trading
        {
            JobId = mjp.JobId.ToString(),
            ClientId = mjp.ClientInfo.Id,
            Ticker = mjp.StockInfo.Ticker,
            Description = mjp.Description
        };

        await _repo.RegisterTradingAsync(job);
    }

    private async Task HandleAsync(TradingFinished mjf)
    {
        Log.Information("Finish Trading Job: {Id}, {StartTime}, {EndTime}",
            mjf.JobId, mjf.StartTime, mjf.EndTime);

        await _repo.MarkTradingAsFinished(mjf.JobId, mjf.StartTime, mjf.EndTime);
    }

    private async Task HandleAsync(DayHasPassed dhp)
    {
        var jobs = await _repo.GetTradingsToBeInvoicedAsync();
        foreach (var jobsPerClient in jobs.GroupBy(job => job.ClientId))
        {
            DateTime invoiceDate = DateTime.Now;
            string clientId = jobsPerClient.Key;
            Client client = await _repo.GetClientAsync(clientId);
            Invoice invoice = new Invoice
            {
                InvoiceId = $"{invoiceDate.ToString("yyyyMMddhhmmss")}-{clientId.Substring(0, 4)}",
                InvoiceDate = invoiceDate.Date,
                ClientId = client.ClientId,
                JobIds = string.Join('|', jobsPerClient.Select(j => j.JobId))
            };

            StringBuilder specification = new StringBuilder();
            decimal totalAmount = 0;
            foreach (var job in jobsPerClient)
            {
                TimeSpan duration = job.EndTime.Value.Subtract(job.StartTime.Value);
                decimal amount = Math.Round((decimal)duration.TotalHours * HOURLY_RATE, 2);
                totalAmount += amount;
                specification.AppendLine($"{job.EndTime.Value.ToString("dd-MM-yyyy")} : {job.Description} on stock with license {job.Ticker} - Duration: {duration.TotalHours} hour - Amount: &#163; {amount}");
            }
            invoice.Specification = specification.ToString();
            invoice.Amount = totalAmount;

            await SendInvoice(client, invoice);
            await _repo.RegisterInvoiceAsync(invoice);

            Log.Information("Invoice {Id} sent to {Client}", invoice.InvoiceId, client.Name);
        }
    }

    private async Task SendInvoice(Client client, Invoice invoice)
    {
        StringBuilder body = new StringBuilder();

        // top banner
        body.AppendLine("<htm><body style='width: 1150px; font-family: Arial;'>");
        body.AppendLine("<image src='cid:banner.jpg'>");

        body.AppendLine("<table style='width: 100%; border: 0px; font-size: 25pt;'><tr>");
        body.AppendLine("<td>StockDesk Limited</td>");
        body.AppendLine("<td style='text-align: right;'>INVOICE</td>");
        body.AppendLine("</tr></table>");

        body.AppendLine("<hr>");

        // invoice and client details
        body.AppendLine("<table style='width: 100%; border: 0px;'><tr>");

        body.AppendLine("<td width='150px' valign='top'>");
        body.AppendLine("Invoice reference<br/>");
        body.AppendLine("Invoice date<br/>");
        body.AppendLine("Amount<br/>");
        body.AppendLine("Payment due by<br/>");
        body.AppendLine("</td>");

        body.AppendLine("<td valign='top'>");
        body.AppendLine($": {invoice.InvoiceId}<br/>");
        body.AppendLine($": {invoice.InvoiceDate.ToString("dd-MM-yyyy")}<br/>");
        body.AppendLine($": &#163; {invoice.Amount}<br/>");
        body.AppendLine($": {invoice.InvoiceDate.AddDays(30).ToString("dd-MM-yyyy")}<br/>");
        body.AppendLine("</td>");

        body.AppendLine("<td width='50px' valign='top'>");
        body.AppendLine("To:");
        body.AppendLine("</td>");

        body.AppendLine("<td valign='top'>");
        body.AppendLine($"{client.Name}<br/>");
        body.AppendLine($"{client.Address}<br/>");
        body.AppendLine($"{client.PostalCode}<br/>");
        body.AppendLine($"{client.City}<br/>");
        body.AppendLine("</td>");

        body.AppendLine("</tr></table>");

        body.AppendLine("<hr><br/>");

        // body
        body.AppendLine($"Dear {client.Name},<br/><br/>");
        body.AppendLine("Hereby we send you an invoice for executing your commands on trading your Stock(s):<br/>");

        body.AppendLine("<ol>");
        foreach (string specificationLine in invoice.Specification.Split('\n'))
        {
            if (specificationLine.Length > 0)
            {
                body.AppendLine($"<li>{specificationLine}</li>");
            }
        }
        body.AppendLine("</ol>");


        body.AppendLine($"Total amount : &#163; {invoice.Amount}<br/><br/>");

        body.AppendLine("Payment terms : Payment within 30 days of invoice date.<br/><br/>");

        // payment details
        body.AppendLine("Payment details<br/><br/>");

        body.AppendLine("<table style='width: 100%; border: 0px;'><tr>");

        body.AppendLine("<td width='120px' valign='top'>");
        body.AppendLine("Bank<br/>");
        body.AppendLine("Name<br/>");
        body.AppendLine("IBAN<br/>");
        body.AppendLine($"Reference<br/>");
        body.AppendLine("</td>");

        body.AppendLine("<td valign='top'>");
        body.AppendLine(": HSBC<br/>");
        body.AppendLine(": StockDesk Limited<br/>");
        body.AppendLine(": IBAN0001234567<br/>");
        body.AppendLine($": {invoice.InvoiceId}<br/>");
        body.AppendLine("</td>");

        body.AppendLine("</tr></table><br/>");

        // greetings
        body.AppendLine("Greetings,<br/><br/>");
        body.AppendLine("StockDesk Helpdesk<br/>");

        body.AppendLine("</htm></body>");

        MailMessage mailMessage = new MailMessage
        {
            From = new MailAddress("invoicing@StockDesk.com"),
            Subject = $"StockDesk Invoice #{invoice.InvoiceId}"
        };
        mailMessage.To.Add("Hardcode-client@StockDesk.com");

        mailMessage.Body = body.ToString();
        mailMessage.IsBodyHtml = true;

        Attachment bannerImage = new Attachment(@"Assets/banner.jpg");
        string contentID = "banner.jpg";
        bannerImage.ContentId = contentID;
        bannerImage.ContentDisposition.Inline = true;
        bannerImage.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
        mailMessage.Attachments.Add(bannerImage);

        await _emailCommunicator.SendEmailAsync(mailMessage);
    }
}
