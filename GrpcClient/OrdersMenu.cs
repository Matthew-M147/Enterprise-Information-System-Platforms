using Google.Protobuf.WellKnownTypes;
using TravelAgency.Grpc;

public class OrdersMenu : CrudMenu
{
    private readonly OrderService.OrderServiceClient _service;
    private readonly ClientService.ClientServiceClient _clients;
    private readonly TourService.TourServiceClient _tours;

    public OrdersMenu(OrderService.OrderServiceClient service, ClientService.ClientServiceClient clients, TourService.TourServiceClient tours)
    {
        _service = service;
        _clients = clients;
        _tours = tours;
    }

    protected override string Title => "Замовлення";

    protected override async Task ShowAllAsync()
    {
        var reply = await _service.GetAllAsync(new Empty());

        Console.WriteLine($"{"№",-4}{"Клієнт",-22}{"Тур",-22}{"Дата поїздки",-14}{"Днів",-6}{"К-сть",-7}{"Ціна",-12}{"Знижка",-8}{"Сума"}");
        Console.WriteLine(new string('-', 110));
        foreach (var o in reply.Orders)
        {
            var tripDate = DateOnly.Parse(o.TripDate);
            Console.WriteLine(
                $"{o.Id,-4}" +
                $"{o.ClientName,-22}" +
                $"{o.TourName,-22}" +
                $"{tripDate,-14:dd.MM.yyyy}" +
                $"{o.DurationDays,-6}" +
                $"{o.Quantity,-7}" +
                $"{o.Price,-12:N2}" +
                $"{o.DiscountPercent.ToString("0.##") + "%",-8}" +
                $"{o.Total:N2}");
        }
    }

    protected override async Task CreateAsync()
    {
        await PrintChoicesAsync();
        var today = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd");
        var order = new OrderMessage { OrderDate = today };
        var created = await _service.CreateAsync(Fill(order, isNew: true));
        Console.WriteLine($"Замовлення №{created.Id} додано: {created.ClientName}, «{created.TourName}», сума {created.Total:N2} грн");
    }

    protected override async Task EditAsync()
    {
        int id = ConsoleInput.ReadInt("№ замовлення");
        var order = await _service.GetAsync(new IdRequest { Id = id });

        await PrintChoicesAsync();
        Console.WriteLine("Enter — залишити поточне значення.");
        var updated = await _service.UpdateAsync(Fill(order, isNew: false));
        Console.WriteLine($"Збережено: {updated.ClientName}, «{updated.TourName}», сума {updated.Total:N2} грн");
    }

    protected override async Task DeleteAsync()
    {
        int id = ConsoleInput.ReadInt("№ замовлення");
        var order = await _service.GetAsync(new IdRequest { Id = id });

        if (!ConsoleInput.Confirm($"Видалити замовлення №{order.Id} ({order.ClientName}, «{order.TourName}»)?"))
            return;

        await _service.DeleteAsync(new IdRequest { Id = id });
        Console.WriteLine("Замовлення видалено.");
    }

    private async Task PrintChoicesAsync()
    {
        var clients = await _clients.GetAllAsync(new Empty());
        var tours = await _tours.GetAllAsync(new Empty());

        Console.WriteLine("Клієнти: " + string.Join(", ", clients.Clients.Select(c => $"{c.Id} — {c.LastName} {c.FirstName}")));
        Console.WriteLine("Тури: " + string.Join(", ", tours.Tours.Select(t => $"{t.Id} — {t.Name}")));
    }

    private static OrderMessage Fill(OrderMessage o, bool isNew)
    {
        o.ClientId = ConsoleInput.ReadInt("ID клієнта", isNew ? null : o.ClientId);
        o.TourId = ConsoleInput.ReadInt("ID туру", isNew ? null : o.TourId);
        o.OrderDate = ConsoleInput.ReadDate("Дата замовлення", o.OrderDate);
        o.TripDate = ConsoleInput.ReadDate("Дата поїздки", isNew ? null : o.TripDate);
        o.Quantity = ConsoleInput.ReadInt("Кількість", isNew ? 1 : o.Quantity);
        o.DiscountPercent = (double)ConsoleInput.ReadDecimal("Знижка, %", isNew ? 0 : (decimal)o.DiscountPercent);
        return o;
    }
}
