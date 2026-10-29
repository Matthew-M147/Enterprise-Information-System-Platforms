using System.Globalization;
using Lab_1.Data;
using Lab_1.Pages;
using Microsoft.EntityFrameworkCore;

namespace Lab_1;

public static class MauiProgram
{
    private const string ConnectionString =
        "Host=localhost;Port=5432;Username=postgres;Password=admin;Database=travel_agency";

    public static MauiApp CreateMauiApp()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddDbContextFactory<TravelAgencyContext>(options => options.UseNpgsql(ConnectionString));

        builder.Services.AddTransient<ClientsPage>();
        builder.Services.AddTransient<ToursPage>();
        builder.Services.AddTransient<OrdersPage>();

        return builder.Build();
    }
}
