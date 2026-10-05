using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/clients")]
public class ClientsController : ControllerBase
{
    private static readonly Expression<Func<Client, ClientDto>> AsDto = c => new ClientDto(
        c.Id, c.LastName, c.FirstName, c.MiddleName, c.Phone,
        c.City, c.Street, c.Building, c.Apartment, c.Orders.Count);

    private readonly TravelAgencyContext _db;

    public ClientsController(TravelAgencyContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<List<ClientDto>> GetAll()
    {
        return await _db.Clients.OrderBy(c => c.Id).Select(AsDto).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> Get(int id)
    {
        var client = await FindDtoAsync(id);
        if (client == null)
            return NotFound();
        return client;
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create(Client client)
    {
        client.Id = 0;
        await CheckPhoneIsUniqueAsync(client);
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = client.Id }, await FindDtoAsync(client.Id));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Client client)
    {
        if (!await _db.Clients.AnyAsync(c => c.Id == id))
            return NotFound();

        client.Id = id;
        await CheckPhoneIsUniqueAsync(client);
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        _db.Clients.Update(client);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var client = await _db.Clients
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (client == null)
            return NotFound();

        _db.Clients.Remove(client);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private Task<ClientDto?> FindDtoAsync(int id)
    {
        return _db.Clients.Where(c => c.Id == id).Select(AsDto).FirstOrDefaultAsync();
    }

    private async Task CheckPhoneIsUniqueAsync(Client client)
    {
        bool exists = await _db.Clients.AnyAsync(c => c.Phone == client.Phone && c.Id != client.Id);
        if (exists)
            ModelState.AddModelError(nameof(Client.Phone), "Клієнт з таким телефоном вже існує");
    }
}
