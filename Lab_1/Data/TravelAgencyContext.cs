using Microsoft.EntityFrameworkCore;

public class TravelAgencyContext : DbContext
{
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=admin;Database=travel_agency");
    }
}
