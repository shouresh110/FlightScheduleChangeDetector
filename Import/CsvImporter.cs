using CsvHelper;
using FlightScheduleChangeDetector.Data;
using FlightScheduleChangeDetector.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FlightScheduleChangeDetector.Import;

public class CsvImporter
{
    private readonly FlightScheduleDbContext _context;

    public CsvImporter(FlightScheduleDbContext context)
    {
        _context = context;
    }

    public void ImportAll(string basePath)
    {
        if (_context.Routes.Any())
            Console.WriteLine("Routes already imported, skipping...");
        else
            ImportRoutes(Path.Combine(basePath, "routes.csv"));


        if (_context.Flights.Any())
            Console.WriteLine("Flights already imported, skipping...");
        else
            ImportFlights(Path.Combine(basePath, "flights.csv"));


        if (_context.Subscriptions.Any())
            Console.WriteLine("Subscriptions already imported, skipping...");
        else
            ImportSubscriptions(Path.Combine(basePath, "subscriptions.csv"));

    }

    private void ImportRoutes(string filePath)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Routes ON");

            _context.Database.ExecuteSqlRaw("DELETE FROM Routes");

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<Route>().ToList();
            _context.Routes.AddRange(records);
            _context.SaveChanges();

            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Routes OFF");

            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    private void ImportFlights(string filePath)
    {
        var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null
        };

        using var transaction = _context.Database.BeginTransaction();

        try
        {
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Flights ON");

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            var records = csv.GetRecords<Flight>().ToList();

            foreach (var record in records)
            {
                record.Route = null;
            }

            _context.Flights.AddRange(records);
            _context.SaveChanges();

            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Flights OFF");

            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    private void ImportSubscriptions(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<Subscription>().ToList();
        _context.Subscriptions.AddRange(records);
        _context.SaveChanges();
    }
}
