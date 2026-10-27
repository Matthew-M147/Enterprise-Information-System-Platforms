using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TravelAgency.Grpc;

public class ClientGrpcService : ClientService.ClientServiceBase
{
    private readonly TravelAgencyContext _db;

    public ClientGrpcService(TravelAgencyContext db)
    {
        _db = db;
    }

    public override async Task<ClientList> GetAll(Empty request, ServerCallContext context)
    {
        var clients = await _db.Clients
            .Include(c => c.Orders)
            .OrderBy(c => c.Id)
            .AsNoTracking()
            .ToListAsync();

        var reply = new ClientList();
        reply.Clients.AddRange(clients.Select(ToMessage));
        return reply;
    }

    public override async Task<ClientMessage> Get(IdRequest request, ServerCallContext context)
    {
        return ToMessage(await FindWithOrdersAsync(request.Id));
    }

    public override async Task<ClientMessage> Create(ClientMessage request, ServerCallContext context)
    {
        var client = FromMessage(request);
        client.Id = 0;
        await ValidateAsync(client);

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
        return ToMessage(client);
    }

    public override async Task<ClientMessage> Update(ClientMessage request, ServerCallContext context)
    {
        if (!await _db.Clients.AnyAsync(c => c.Id == request.Id))
            throw ModelValidator.NotFound($"Клієнта з id={request.Id} не знайдено");

        var client = FromMessage(request);
        await ValidateAsync(client);

        _db.Clients.Update(client);
        await _db.SaveChangesAsync();
        return ToMessage(await FindWithOrdersAsync(client.Id));
    }

    public override async Task<Empty> Delete(IdRequest request, ServerCallContext context)
    {
        var client = await FindWithOrdersAsync(request.Id);
        _db.Clients.Remove(client);
        await _db.SaveChangesAsync();
        return new Empty();
    }

    private async Task<Client> FindWithOrdersAsync(int id)
    {
        return await _db.Clients.Include(c => c.Orders).FirstOrDefaultAsync(c => c.Id == id)
            ?? throw ModelValidator.NotFound($"Клієнта з id={id} не знайдено");
    }

    private async Task ValidateAsync(Client client)
    {
        var errors = ModelValidator.Validate(client);
        if (await _db.Clients.AnyAsync(c => c.Phone == client.Phone && c.Id != client.Id))
            errors.Add("Клієнт з таким телефоном вже існує");
        ModelValidator.ThrowIfInvalid(errors);
    }

    private static ClientMessage ToMessage(Client c)
    {
        var message = new ClientMessage
        {
            Id = c.Id,
            LastName = c.LastName,
            FirstName = c.FirstName,
            Phone = c.Phone,
            City = c.City,
            Street = c.Street,
            Building = c.Building,
            OrdersCount = c.Orders.Count
        };
        if (c.MiddleName != null)
            message.MiddleName = c.MiddleName;
        if (c.Apartment != null)
            message.Apartment = c.Apartment;
        return message;
    }

    private static Client FromMessage(ClientMessage m)
    {
        return new Client
        {
            Id = m.Id,
            LastName = m.LastName.Trim(),
            FirstName = m.FirstName.Trim(),
            MiddleName = m.HasMiddleName && !string.IsNullOrWhiteSpace(m.MiddleName) ? m.MiddleName.Trim() : null,
            Phone = m.Phone.Trim(),
            City = m.City.Trim(),
            Street = m.Street.Trim(),
            Building = m.Building.Trim(),
            Apartment = m.HasApartment && !string.IsNullOrWhiteSpace(m.Apartment) ? m.Apartment.Trim() : null
        };
    }
}
