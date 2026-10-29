using System.Globalization;
using Lab_1.Data;
using Lab_1.Models;
using Lab_1.Services;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages;

public partial class TourEditPage : ContentPage
{
    private readonly IDbContextFactory<TravelAgencyContext> _dbFactory;
    private readonly int? _id;
    private bool _loaded;

    public TourEditPage(IDbContextFactory<TravelAgencyContext> dbFactory, int? id)
    {
        InitializeComponent();
        _dbFactory = dbFactory;
        _id = id;
        Title = id == null ? "Новий тур" : "Редагування туру";
        if (id == null)
            DurationEntry.Text = "1";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_loaded || _id == null)
            return;
        _loaded = true;

        await using var db = await _dbFactory.CreateDbContextAsync();
        var tour = await db.Tours.FindAsync(_id.Value);
        if (tour == null)
        {
            await DisplayAlertAsync("Помилка", "Тур не знайдено", "OK");
            await Navigation.PopAsync();
            return;
        }

        NameEntry.Text = tour.Name;
        PriceEntry.Text = tour.Price.ToString(CultureInfo.InvariantCulture);
        DurationEntry.Text = tour.DurationDays.ToString();
        DescriptionEditor.Text = tour.Description;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var tour = new Tour
        {
            Id = _id ?? 0,
            Name = NameEntry.TrimmedText(),
            Price = PriceEntry.ToDecimal(),
            DurationDays = DurationEntry.ToInt(),
            Description = DescriptionEditor.Text?.Trim() ?? ""
        };

        await using var db = await _dbFactory.CreateDbContextAsync();
        var errors = ModelValidator.Validate(tour);
        if (await db.Tours.AnyAsync(t => t.Name == tour.Name && t.Id != tour.Id))
            errors.Add("Тур з такою назвою вже існує");
        if (errors.Count > 0)
        {
            ErrorLabel.Text = string.Join(Environment.NewLine, errors);
            ErrorLabel.IsVisible = true;
            return;
        }

        if (_id == null)
            db.Tours.Add(tour);
        else
            db.Tours.Update(tour);
        await db.SaveChangesAsync();
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
