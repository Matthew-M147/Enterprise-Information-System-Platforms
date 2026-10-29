using Lab_1.Data;
using Lab_1.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages;

public partial class ToursPage : ContentPage
{
    private readonly IDbContextFactory<TravelAgencyContext> _dbFactory;

    public ToursPage(IDbContextFactory<TravelAgencyContext> dbFactory)
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
            ToursView.ItemsSource = await db.Tours
                .Include(t => t.Orders)
                .OrderBy(t => t.Id)
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
        await Navigation.PushAsync(new TourEditPage(_dbFactory, null));
    }

    private async void OnEditClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: Tour tour })
            await Navigation.PushAsync(new TourEditPage(_dbFactory, tour.Id));
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: Tour tour })
            return;

        string message = tour.Orders.Count > 0
            ? $"Видалити тур «{tour.Name}» разом із замовленнями ({tour.Orders.Count})?"
            : $"Видалити тур «{tour.Name}»?";
        if (!await DisplayAlertAsync("Видалення", message, "Так", "Ні"))
            return;

        await using var db = await _dbFactory.CreateDbContextAsync();
        var entity = await db.Tours
            .Include(t => t.Orders)
            .FirstOrDefaultAsync(t => t.Id == tour.Id);
        if (entity != null)
        {
            db.Tours.Remove(entity);
            await db.SaveChangesAsync();
        }
        await LoadAsync();
    }
}
