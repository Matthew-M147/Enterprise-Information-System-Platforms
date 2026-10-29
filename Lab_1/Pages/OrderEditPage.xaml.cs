using System.Globalization;
using Lab_1.Data;
using Lab_1.Models;
using Lab_1.Services;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages;

public partial class OrderEditPage : ContentPage
{
    private readonly IDbContextFactory<TravelAgencyContext> _dbFactory;
    private readonly int? _id;
    private bool _loaded;

    public OrderEditPage(IDbContextFactory<TravelAgencyContext> dbFactory, int? id)
    {
        InitializeComponent();
        _dbFactory = dbFactory;
        _id = id;
        Title = id == null ? "Нове замовлення" : $"Редагування замовлення №{id}";
        ClientPicker.ItemDisplayBinding = new Binding(nameof(Client.FullName));
        TourPicker.ItemDisplayBinding = new Binding(nameof(Tour.Name));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_loaded)
            return;
        _loaded = true;

        await using var db = await _dbFactory.CreateDbContextAsync();
        var clients = await db.Clients.OrderBy(c => c.LastName).AsNoTracking().ToListAsync();
        var tours = await db.Tours.OrderBy(t => t.Name).AsNoTracking().ToListAsync();
        ClientPicker.ItemsSource = clients;
        TourPicker.ItemsSource = tours;

        var order = _id == null
            ? new Order { OrderDate = DateOnly.FromDateTime(DateTime.Today), TripDate = DateOnly.FromDateTime(DateTime.Today), Quantity = 1 }
            : await db.Orders.FindAsync(_id.Value);
        if (order == null)
        {
            await DisplayAlertAsync("Помилка", "Замовлення не знайдено", "OK");
            await Navigation.PopAsync();
            return;
        }

        ClientPicker.SelectedItem = clients.FirstOrDefault(c => c.Id == order.ClientId);
        TourPicker.SelectedItem = tours.FirstOrDefault(t => t.Id == order.TourId);
        OrderDatePicker.Date = order.OrderDate.ToDateTime(TimeOnly.MinValue);
        TripDatePicker.Date = order.TripDate.ToDateTime(TimeOnly.MinValue);
        QuantityEntry.Text = order.Quantity.ToString();
        DiscountEntry.Text = order.DiscountPercent.ToString("0.##", CultureInfo.InvariantCulture);
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var order = new Order
        {
            Id = _id ?? 0,
            ClientId = (ClientPicker.SelectedItem as Client)?.Id ?? 0,
            TourId = (TourPicker.SelectedItem as Tour)?.Id ?? 0,
            OrderDate = DateOnly.FromDateTime(OrderDatePicker.Date ?? DateTime.Today),
            TripDate = DateOnly.FromDateTime(TripDatePicker.Date ?? DateTime.Today),
            Quantity = QuantityEntry.ToInt(),
            DiscountPercent = DiscountEntry.ToDecimal()
        };

        var errors = ModelValidator.Validate(order);
        if (order.TripDate < order.OrderDate)
            errors.Add("Дата поїздки не може бути раніше дати замовлення");
        if (errors.Count > 0)
        {
            ErrorLabel.Text = string.Join(Environment.NewLine, errors);
            ErrorLabel.IsVisible = true;
            return;
        }

        await using var db = await _dbFactory.CreateDbContextAsync();
        if (_id == null)
            db.Orders.Add(order);
        else
            db.Orders.Update(order);
        await db.SaveChangesAsync();
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
