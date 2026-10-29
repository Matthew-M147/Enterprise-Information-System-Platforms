using Lab_1.Data;
using Lab_1.Models;
using Lab_1.Services;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages;

public partial class ClientEditPage : ContentPage
{
    private readonly IDbContextFactory<TravelAgencyContext> _dbFactory;
    private readonly int? _id;
    private bool _loaded;

    public ClientEditPage(IDbContextFactory<TravelAgencyContext> dbFactory, int? id)
    {
        InitializeComponent();
        _dbFactory = dbFactory;
        _id = id;
        Title = id == null ? "Новий клієнт" : "Редагування клієнта";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_loaded || _id == null)
            return;
        _loaded = true;

        await using var db = await _dbFactory.CreateDbContextAsync();
        var client = await db.Clients.FindAsync(_id.Value);
        if (client == null)
        {
            await DisplayAlertAsync("Помилка", "Клієнта не знайдено", "OK");
            await Navigation.PopAsync();
            return;
        }

        LastNameEntry.Text = client.LastName;
        FirstNameEntry.Text = client.FirstName;
        MiddleNameEntry.Text = client.MiddleName;
        PhoneEntry.Text = client.Phone;
        CityEntry.Text = client.City;
        StreetEntry.Text = client.Street;
        BuildingEntry.Text = client.Building;
        ApartmentEntry.Text = client.Apartment;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var client = new Client
        {
            Id = _id ?? 0,
            LastName = LastNameEntry.TrimmedText(),
            FirstName = FirstNameEntry.TrimmedText(),
            MiddleName = MiddleNameEntry.OptionalText(),
            Phone = PhoneEntry.TrimmedText(),
            City = CityEntry.TrimmedText(),
            Street = StreetEntry.TrimmedText(),
            Building = BuildingEntry.TrimmedText(),
            Apartment = ApartmentEntry.OptionalText()
        };

        await using var db = await _dbFactory.CreateDbContextAsync();
        var errors = ModelValidator.Validate(client);
        if (await db.Clients.AnyAsync(c => c.Phone == client.Phone && c.Id != client.Id))
            errors.Add("Клієнт з таким телефоном вже існує");
        if (errors.Count > 0)
        {
            ErrorLabel.Text = string.Join(Environment.NewLine, errors);
            ErrorLabel.IsVisible = true;
            return;
        }

        if (_id == null)
            db.Clients.Add(client);
        else
            db.Clients.Update(client);
        await db.SaveChangesAsync();
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
