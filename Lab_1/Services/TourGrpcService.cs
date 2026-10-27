using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TravelAgency.Grpc;

public class TourGrpcService : TourService.TourServiceBase
{
    private readonly TravelAgencyContext _db;

    public TourGrpcService(TravelAgencyContext db)
    {
        _db = db;
    }

    public override async Task<TourList> GetAll(Empty request, ServerCallContext context)
    {
        var tours = await _db.Tours
            .Include(t => t.Orders)
            .OrderBy(t => t.Id)
            .AsNoTracking()
            .ToListAsync();

        var reply = new TourList();
        reply.Tours.AddRange(tours.Select(ToMessage));
        return reply;
    }

    public override async Task<TourMessage> Get(IdRequest request, ServerCallContext context)
    {
        return ToMessage(await FindWithOrdersAsync(request.Id));
    }

    public override async Task<TourMessage> Create(TourMessage request, ServerCallContext context)
    {
        var tour = FromMessage(request);
        tour.Id = 0;
        await ValidateAsync(tour);

        _db.Tours.Add(tour);
        await _db.SaveChangesAsync();
        return ToMessage(tour);
    }

    public override async Task<TourMessage> Update(TourMessage request, ServerCallContext context)
    {
        if (!await _db.Tours.AnyAsync(t => t.Id == request.Id))
            throw ModelValidator.NotFound($"Тур з id={request.Id} не знайдено");

        var tour = FromMessage(request);
        await ValidateAsync(tour);

        _db.Tours.Update(tour);
        await _db.SaveChangesAsync();
        return ToMessage(await FindWithOrdersAsync(tour.Id));
    }

    public override async Task<Empty> Delete(IdRequest request, ServerCallContext context)
    {
        var tour = await FindWithOrdersAsync(request.Id);
        _db.Tours.Remove(tour);
        await _db.SaveChangesAsync();
        return new Empty();
    }

    private async Task<Tour> FindWithOrdersAsync(int id)
    {
        return await _db.Tours.Include(t => t.Orders).FirstOrDefaultAsync(t => t.Id == id)
            ?? throw ModelValidator.NotFound($"Тур з id={id} не знайдено");
    }

    private async Task ValidateAsync(Tour tour)
    {
        var errors = ModelValidator.Validate(tour);
        if (await _db.Tours.AnyAsync(t => t.Name == tour.Name && t.Id != tour.Id))
            errors.Add("Тур з такою назвою вже існує");
        ModelValidator.ThrowIfInvalid(errors);
    }

    private static TourMessage ToMessage(Tour t)
    {
        return new TourMessage
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            Price = (double)t.Price,
            DurationDays = t.DurationDays,
            OrdersCount = t.Orders.Count
        };
    }

    private static Tour FromMessage(TourMessage m)
    {
        return new Tour
        {
            Id = m.Id,
            Name = m.Name.Trim(),
            Description = m.Description.Trim(),
            Price = Math.Round((decimal)m.Price, 2),
            DurationDays = m.DurationDays
        };
    }
}
