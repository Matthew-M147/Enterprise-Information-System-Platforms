using System.Globalization;
using Microsoft.EntityFrameworkCore;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TravelAgencyContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TravelAgency")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapRazorPages();
app.MapControllers();

app.Run();
