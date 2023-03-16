namespace StockDesk.Application.ClientManagementAPI.Controllers;

[Route("/api/[controller]")]
public class ClientsController : Controller
{
    IMessagePublisher _messagePublisher;
    ClientManagementDBContext _dbContext;

    public ClientsController(ClientManagementDBContext dbContext, IMessagePublisher messagePublisher)
    {
        _dbContext = dbContext;
        _messagePublisher = messagePublisher;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _dbContext.Clients.ToListAsync());
    }

    [HttpGet]
    [Route("{clientId}", Name = "GetByClientId")]
    public async Task<IActionResult> GetByClientId(string clientId)
    {
        var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
        if (client == null)
        {
            return NotFound();
        }
        return Ok(client);
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterClient command)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // insert client
                Client client = command.MapToClient();
                _dbContext.Clients.Add(client);
                await _dbContext.SaveChangesAsync();

                // send event
                ClientRegistered e = command.MapToClientRegistered();
                await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");

                // return result
                return CreatedAtRoute("GetByClientId", new { clientId = client.ClientId }, client);
            }
            return BadRequest();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Unable to save changes. " +
                "Try again, and if the problem persists " +
                "see your system administrator.");
            return StatusCode(StatusCodes.Status500InternalServerError);
            throw;
        }
    }
}