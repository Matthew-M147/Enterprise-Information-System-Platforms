using System.Globalization;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TravelAgency.Grpc;

public class OrderGrpcService : OrderService.OrderServiceBase
{
    private const string DateFormat = "yyyy-MM-dd";

    private readonly TravelAgencyContext _db;

    public OrderGrpcService(TravelAgencyContext db)
    {
        _db = db;
    }

    public override async Task<OrderList> GetAll(Empty request, ServerCallContext context)
    {
        var orders = await _db.Orders
            .Include(o => o.Client)
            .Include(o => o.Tour)
            .OrderBy(o => o.ClientId)
            .ThenBy(o => o.TripDate)
            .AsNoTracking()
            .ToListAsync();

        var reply = new OrderList();
        reply.Orders.AddRange(orders.Select(ToMessage));
        return reply;
    }

    public override async Task<OrderMessage> Get(IdRequest request, ServerCallContext context)
    {
        return ToMessage(await FindWithDetailsAsync(request.Id));
    }

    public override async Task<OrderMessage> Create(OrderMessage request, ServerCallContext context)
    {
        var order = FromMessage(request);
        order.Id = 0;
        await ValidateAsync(order);

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return ToMessage(await FindWithDetailsAsync(order.Id));
    }

    public override async Task<OrderMessage> Update(OrderMessage request, ServerCallContext context)
    {
        if (!await _db.Orders.AnyAsync(o => o.Id == request.Id))
            throw ModelValidator.NotFound($"Замовлення з id={request.Id} не знайдено");

        var order = FromMessage(request);
        await ValidateAsync(order);

        _db.Orders.Update(order);
        await _db.SaveChangesAsync();
        return ToMessage(await FindWithDetailsAsync(order.Id));
    }

    public override async Task<Empty> Delete(IdRequest request, ServerCallContext context)
    {
        var order = await _db.Orders.FindAsync(request.Id)
            ?? throw ModelValidator.NotFound($"Замовлення з id={request.Id} не знайдено");
        _db.Orders.Remove(order);
        await _db.SaveChangesAsync();
        return new Empty();
    }

    private async Task<Order> FindWithDetailsAsync(int id)
    {
        return await _db.Orders
            .Include(o => o.Client)
            .Include(o => o.Tour)
            .FirstOrDefaultAsync(o => o.Id == id)
            ?? throw ModelValidator.NotFound($"Замовлення з id={id} не знайдено");
    }

    private async Task ValidateAsync(Order order)
    {
        var errors = ModelValidator.Validate(order);
        if (!await _db.Clients.AnyAsync(c => c.Id == order.ClientId))
            errors.Add("Клієнта не знайдено");
        if (!await _db.Tours.AnyAsync(t => t.Id == order.TourId))
            errors.Add("Тур не знайдено");
        if (order.TripDate < order.OrderDate)
            errors.Add("Дата поїздки не може бути раніше дати замовлення");
        ModelValidator.ThrowIfInvalid(errors);
    }

    private static OrderMessage ToMessage(Order o)
    {
        return new OrderMessage
        {
            Id = o.Id,
            ClientId = o.ClientId,
            ClientName = $"{o.Client.LastName} {o.Client.FirstName}",
            TourId = o.TourId,
            TourName = o.Tour.Name,
            OrderDate = o.OrderDate.ToString(DateFormat, CultureInfo.InvariantCulture),
            TripDate = o.TripDate.ToString(DateFormat, CultureInfo.InvariantCulture),
            DurationDays = o.Tour.DurationDays,
            Quantity = o.Quantity,
            Price = (double)o.Tour.Price,
            DiscountPercent = (double)o.DiscountPercent,
            Total = (double)o.Total
        };
    }

    private static Order FromMessage(OrderMessage m)
    {
        return new Order
        {
            Id = m.Id,
            ClientId = m.ClientId,
            TourId = m.TourId,
            OrderDate = ParseDate(m.OrderDate, "дата замовлення"),
            TripDate = ParseDate(m.TripDate, "дата поїздки"),
            Quantity = m.Quantity,
            DiscountPercent = Math.Round((decimal)m.DiscountPercent, 2)
        };
    }

    private static DateOnly ParseDate(string value, string field)
    {
        if (DateOnly.TryParseExact(value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;
        throw new RpcException(new Status(StatusCode.InvalidArgument, $"Некоректна {field}: очікується формат {DateFormat}"));
    }
}
