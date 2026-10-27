using System.Globalization;
using System.Text;
using Grpc.Net.Client;
using TravelAgency.Grpc;

Console.OutputEncoding = Encoding.UTF8;
if (OperatingSystem.IsWindows() && !Console.IsInputRedirected)
    Console.InputEncoding = Encoding.Unicode;
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

string address = args.Length > 0 ? args[0] : "http://localhost:5080";
using var channel = GrpcChannel.ForAddress(address);

var clientService = new ClientService.ClientServiceClient(channel);
var tourService = new TourService.TourServiceClient(channel);
var orderService = new OrderService.OrderServiceClient(channel);

var clientsMenu = new ClientsMenu(clientService);
var toursMenu = new ToursMenu(tourService);
var ordersMenu = new OrdersMenu(orderService, clientService, tourService);

Console.WriteLine($"gRPC-клієнт туристичної фірми. Сервер: {address}");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("===== Головне меню =====");
    Console.WriteLine("1. Клієнти");
    Console.WriteLine("2. Тури");
    Console.WriteLine("3. Замовлення");
    Console.WriteLine("0. Вихід");

    switch (ConsoleInput.ReadChoice())
    {
        case "1": await clientsMenu.RunAsync(); break;
        case "2": await toursMenu.RunAsync(); break;
        case "3": await ordersMenu.RunAsync(); break;
        case "0": return;
        default: Console.WriteLine("Невідомий пункт меню."); break;
    }
}
