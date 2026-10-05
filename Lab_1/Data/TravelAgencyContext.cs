using Microsoft.EntityFrameworkCore;

public class TravelAgencyContext : DbContext
{
    public TravelAgencyContext(DbContextOptions<TravelAgencyContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<Order> Orders => Set<Order>();
}
