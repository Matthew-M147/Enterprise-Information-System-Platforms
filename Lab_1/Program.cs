using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddDbContext<TravelAgencyContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TravelAgency")));

var app = builder.Build();

app.MapGrpcService<ClientGrpcService>();
app.MapGrpcService<TourGrpcService>();
app.MapGrpcService<OrderGrpcService>();

app.Run();
