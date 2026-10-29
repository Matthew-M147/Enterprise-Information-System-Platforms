using Lab_1.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Data;

public class TravelAgencyContext : DbContext
{
    public TravelAgencyContext(DbContextOptions<TravelAgencyContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<Order> Orders => Set<Order>();
}
