using Lab_1.Data;
using Lab_1.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages;

public partial class OrdersPage : ContentPage
{
    private readonly IDbContextFactory<TravelAgencyContext> _dbFactory;

    public OrdersPage(IDbContextFactory<TravelAgencyContext> dbFactory)
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
            OrdersView.ItemsSource = await db.Orders
                .Include(o => o.Client)
                .Include(o => o.Tour)
                .OrderBy(o => o.ClientId)
                .ThenBy(o => o.TripDate)
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
        await Navigation.PushAsync(new OrderEditPage(_dbFactory, null));
    }

    private async void OnEditClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: Order order })
            await Navigation.PushAsync(new OrderEditPage(_dbFactory, order.Id));
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: Order order })
            return;

        string message = $"Видалити замовлення №{order.Id} ({order.Client.LastName} {order.Client.FirstName}, «{order.Tour.Name}»)?";
        if (!await DisplayAlertAsync("Видалення", message, "Так", "Ні"))
            return;

        await using var db = await _dbFactory.CreateDbContextAsync();
        var entity = await db.Orders.FindAsync(order.Id);
        if (entity != null)
        {
            db.Orders.Remove(entity);
            await db.SaveChangesAsync();
        }
        await LoadAsync();
    }
}
