namespace StockDesk.InvoiceService.CommunicationChannels;

public interface IEmailCommunicator
{
    Task SendEmailAsync(MailMessage message);
}