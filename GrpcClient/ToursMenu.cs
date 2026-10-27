using Google.Protobuf.WellKnownTypes;
using TravelAgency.Grpc;

public class ToursMenu : CrudMenu
{
    private readonly TourService.TourServiceClient _service;

    public ToursMenu(TourService.TourServiceClient service)
    {
        _service = service;
    }

    protected override string Title => "Тури";

    protected override async Task ShowAllAsync()
    {
        var reply = await _service.GetAllAsync(new Empty());

        foreach (var t in reply.Tours)
        {
            Console.WriteLine($"[{t.Id}] {t.Name}");
            Console.WriteLine($"    Ціна: {t.Price:N2} грн, тривалість: {t.DurationDays} дн., замовлень: {t.OrdersCount}");
            Console.WriteLine($"    Опис: {t.Description}");
        }
    }

    protected override async Task CreateAsync()
    {
        var tour = Fill(new TourMessage(), isNew: true);
        var created = await _service.CreateAsync(tour);
        Console.WriteLine($"Тур додано: id={created.Id}, {created.Name}");
    }

    protected override async Task EditAsync()
    {
        int id = ConsoleInput.ReadInt("ID туру");
        var tour = await _service.GetAsync(new IdRequest { Id = id });

        Console.WriteLine("Enter — залишити поточне значення.");
        var updated = await _service.UpdateAsync(Fill(tour, isNew: false));
        Console.WriteLine($"Збережено: {updated.Name}, {updated.Price:N2} грн, {updated.DurationDays} дн.");
    }

    protected override async Task DeleteAsync()
    {
        int id = ConsoleInput.ReadInt("ID туру");
        var tour = await _service.GetAsync(new IdRequest { Id = id });

        string question = tour.OrdersCount > 0
            ? $"Видалити тур «{tour.Name}» разом із замовленнями ({tour.OrdersCount})?"
            : $"Видалити тур «{tour.Name}»?";
        if (!ConsoleInput.Confirm(question))
            return;

        await _service.DeleteAsync(new IdRequest { Id = id });
        Console.WriteLine("Тур видалено.");
    }

    private static TourMessage Fill(TourMessage t, bool isNew)
    {
        t.Name = ConsoleInput.ReadText("Назва", isNew ? null : t.Name);
        t.Description = ConsoleInput.ReadText("Опис", isNew ? null : t.Description);
        t.Price = (double)ConsoleInput.ReadDecimal("Ціна, грн", isNew ? null : (decimal)t.Price);
        t.DurationDays = ConsoleInput.ReadInt("Тривалість, днів", isNew ? null : t.DurationDays);
        return t;
    }
}
