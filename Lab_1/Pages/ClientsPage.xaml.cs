using Lab_1.Data;
using Lab_1.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages;

public partial class ClientsPage : ContentPage
{
    private readonly IDbContextFactory<TravelAgencyContext> _dbFactory;

    public ClientsPage(IDbContextFactory<TravelAgencyContext> dbFactory)
    {
        InitializeComponent();
        _dbFactory = dbFactory;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            ClientsView.ItemsSource = await db.Clients
                .Include(c => c.Orders)
                .OrderBy(c => c.Id)
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Помилка бази даних", ex.Message, "OK");
        }
    }

    private async void OnAddClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new ClientEditPage(_dbFactory, null));
    }

    private async void OnEditClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: Client client })
            await Navigation.PushAsync(new ClientEditPage(_dbFactory, client.Id));
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: Client client })
            return;

        string message = client.Orders.Count > 0
            ? $"Видалити {client.FullName} разом із замовленнями ({client.Orders.Count})?"
            : $"Видалити {client.FullName}?";
        if (!await DisplayAlertAsync("Видалення", message, "Так", "Ні"))
            return;

        await using var db = await _dbFactory.CreateDbContextAsync();
        var entity = await db.Clients
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == client.Id);
        if (entity != null)
        {
            db.Clients.Remove(entity);
            await db.SaveChangesAsync();
        }
        await LoadAsync();
    }
}
