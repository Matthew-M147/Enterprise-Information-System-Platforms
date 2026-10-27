using Google.Protobuf.WellKnownTypes;
using TravelAgency.Grpc;

public class ClientsMenu : CrudMenu
{
    private readonly ClientService.ClientServiceClient _service;

    public ClientsMenu(ClientService.ClientServiceClient service)
    {
        _service = service;
    }

    protected override string Title => "Клієнти";

    protected override async Task ShowAllAsync()
    {
        var reply = await _service.GetAllAsync(new Empty());

        Console.WriteLine($"{"ID",-4}{"ПІБ",-38}{"Телефон",-16}{"Замовлень",-11}{"Адреса"}");
        Console.WriteLine(new string('-', 120));
        foreach (var c in reply.Clients)
            Console.WriteLine($"{c.Id,-4}{FullName(c),-38}{c.Phone,-16}{c.OrdersCount,-11}{Address(c)}");
    }

    protected override async Task CreateAsync()
    {
        var client = Fill(new ClientMessage(), isNew: true);
        var created = await _service.CreateAsync(client);
        Console.WriteLine($"Клієнта додано: id={created.Id}, {FullName(created)}");
    }

    protected override async Task EditAsync()
    {
        int id = ConsoleInput.ReadInt("ID клієнта");
        var client = await _service.GetAsync(new IdRequest { Id = id });

        Console.WriteLine("Enter — залишити поточне значення.");
        var updated = await _service.UpdateAsync(Fill(client, isNew: false));
        Console.WriteLine($"Збережено: {FullName(updated)}, {Address(updated)}");
    }

    protected override async Task DeleteAsync()
    {
        int id = ConsoleInput.ReadInt("ID клієнта");
        var client = await _service.GetAsync(new IdRequest { Id = id });

        string question = client.OrdersCount > 0
            ? $"Видалити {FullName(client)} разом із замовленнями ({client.OrdersCount})?"
            : $"Видалити {FullName(client)}?";
        if (!ConsoleInput.Confirm(question))
            return;

        await _service.DeleteAsync(new IdRequest { Id = id });
        Console.WriteLine("Клієнта видалено.");
    }

    private static ClientMessage Fill(ClientMessage c, bool isNew)
    {
        c.LastName = ConsoleInput.ReadText("Прізвище", isNew ? null : c.LastName);
        c.FirstName = ConsoleInput.ReadText("Ім'я", isNew ? null : c.FirstName);
        SetMiddleName(c, ConsoleInput.ReadOptionalText("По батькові", c.HasMiddleName ? c.MiddleName : null));
        c.Phone = ConsoleInput.ReadText("Телефон", isNew ? null : c.Phone);
        c.City = ConsoleInput.ReadText("Місто", isNew ? null : c.City);
        c.Street = ConsoleInput.ReadText("Вулиця", isNew ? null : c.Street);
        c.Building = ConsoleInput.ReadText("Будинок", isNew ? null : c.Building);
        SetApartment(c, ConsoleInput.ReadOptionalText("Квартира", c.HasApartment ? c.Apartment : null));
        return c;
    }

    private static void SetMiddleName(ClientMessage c, string? value)
    {
        if (string.IsNullOrEmpty(value))
            c.ClearMiddleName();
        else
            c.MiddleName = value;
    }

    private static void SetApartment(ClientMessage c, string? value)
    {
        if (string.IsNullOrEmpty(value))
            c.ClearApartment();
        else
            c.Apartment = value;
    }

    private static string FullName(ClientMessage c) => $"{c.LastName} {c.FirstName} {c.MiddleName}".Trim();

    private static string Address(ClientMessage c) =>
        $"{c.City}, {c.Street}, {c.Building}" + (c.HasApartment ? $", кв. {c.Apartment}" : "");
}
