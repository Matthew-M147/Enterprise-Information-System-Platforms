using System.Text;
using Microsoft.EntityFrameworkCore;

Console.OutputEncoding = Encoding.UTF8;

using (var db = new TravelAgencyContext())
{
    Console.WriteLine("Підключення до бази даних успішне!\n");

    ShowClients(db);
    ShowTours(db);
    ShowOrders(db);
}

void ShowClients(TravelAgencyContext db)
{
    var clients = db.Clients.OrderBy(c => c.Id).ToList();

    Console.WriteLine("===== Клієнти =====");
    Console.WriteLine($"{"ID",-4}{"ПІБ",-38}{"Телефон",-16}{"Адреса"}");
    Console.WriteLine(new string('-', 110));

    foreach (var c in clients)
    {
        Console.WriteLine($"{c.Id,-4}{c.FullName,-38}{c.Phone,-16}{c.Address}");
    }
    Console.WriteLine();
}

void ShowTours(TravelAgencyContext db)
{
    var tours = db.Tours.OrderBy(t => t.Id).ToList();

    Console.WriteLine("===== Тури =====");

    foreach (var t in tours)
    {
        Console.WriteLine($"[{t.Id}] {t.Name}");
        Console.WriteLine($"    Ціна: {t.Price:N2} грн, тривалість: {t.DurationDays} дн.");
        Console.WriteLine($"    Опис: {t.Description}");
    }
    Console.WriteLine();
}

void ShowOrders(TravelAgencyContext db)
{
    var orders = db.Orders
        .Include(o => o.Client)
        .Include(o => o.Tour)
        .OrderBy(o => o.ClientId)
        .ThenBy(o => o.TripDate)
        .ToList();

    Console.WriteLine("===== Замовлення =====");
    Console.WriteLine($"{"№",-4}{"Клієнт",-22}{"Тур",-22}{"Дата поїздки",-14}{"Днів",-6}{"К-сть",-7}{"Ціна",-12}{"Знижка",-8}{"Сума"}");
    Console.WriteLine(new string('-', 110));

    foreach (var o in orders)
    {
        string client = $"{o.Client.LastName} {o.Client.FirstName}";
        Console.WriteLine(
            $"{o.Id,-4}" +
            $"{client,-22}" +
            $"{o.Tour.Name,-22}" +
            $"{o.TripDate,-14:dd.MM.yyyy}" +
            $"{o.Tour.DurationDays,-6}" +
            $"{o.Quantity,-7}" +
            $"{o.Tour.Price,-12:N2}" +
            $"{o.DiscountPercent.ToString("0.##") + "%",-8}" +
            $"{o.Total:N2}");
    }
}
