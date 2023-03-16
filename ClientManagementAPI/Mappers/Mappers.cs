namespace StockDesk.ClientManagementAPI.Mappers;

public static class Mappers
{
    public static ClientRegistered MapToClientRegistered(this RegisterClient command) => new ClientRegistered
    (
        System.Guid.NewGuid(),
        command.ClientId,
        command.Name,
        command.Address,
        command.PostalCode,
        command.City,
        command.TelephoneNumber,
        command.EmailAddress
    );

    public static Client MapToClient(this RegisterClient command) => new Client
    {
        ClientId = command.ClientId,
        Name = command.Name,
        Address = command.Address,
        PostalCode = command.PostalCode,
        City = command.City,
        TelephoneNumber = command.TelephoneNumber,
        EmailAddress = command.EmailAddress
    };
}