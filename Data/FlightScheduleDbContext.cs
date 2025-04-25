using FlightScheduleChangeDetector.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightScheduleChangeDetector.Data;

public class FlightScheduleDbContext : DbContext
{
    public DbSet<Route> Routes { get; set; }
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Data Source=PRODUCTION\SQL2022;Initial Catalog=FlightScheduleDB;uid=sa;pwd=admin@123;TrustServerCertificate=True");

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscription>().HasKey(s => new { s.AgencyId, s.OriginCityId, s.DestinationCityId });
    }
}