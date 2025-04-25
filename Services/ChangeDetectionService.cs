using FlightScheduleChangeDetector.Data;
using FlightScheduleChangeDetector.Models;
using CsvHelper;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace FlightScheduleChangeDetector.Services;

public class ChangeDetectionService
{
    private readonly FlightScheduleDbContext _context;

    public ChangeDetectionService(FlightScheduleDbContext context)
    {
        _context = context;
    }

    public void DetectChanges(DateTime startDate, DateTime endDate, int agencyId)
    {
        // 1. Get subscriptions for the given agency
        var subscriptions = _context.Subscriptions
            .Where(s => s.AgencyId == agencyId)
            .ToList();

        if (!subscriptions.Any())
        {
            Console.WriteLine("No subscriptions found for the given agency.");
            return;
        }

        // 2. Filter flights based on date range and agency subscriptions
        var flights = _context.Flights
            .Include(f => f.Route) // مهم!
            .Where(f => f.DepartureTime.Date >= startDate.Date && f.DepartureTime.Date <= endDate.Date)
            .ToList()
            .Where(f => subscriptions.Any(s =>
                s.OriginCityId == f.Route.OriginCityId &&
                s.DestinationCityId == f.Route.DestinationCityId))
            .ToList();

        if (!flights.Any())
        {
            Console.WriteLine("No flights found for the given criteria.");
            return;
        }

        var results = new List<ChangeResult>();

        foreach (var flight in flights)
        {
            // Calculate previous and next week target times
            var previousWeekTarget = flight.DepartureTime.AddDays(-7);
            var nextWeekTarget = flight.DepartureTime.AddDays(7);

            // Find flights 7 days before within +/-30 minutes
            bool hasPreviousFlight = flights.Any(f =>
                f.AirlineId == flight.AirlineId &&
                f.Route.OriginCityId == flight.Route.OriginCityId &&
                f.Route.DestinationCityId == flight.Route.DestinationCityId &&
                Math.Abs((f.DepartureTime - previousWeekTarget).TotalMinutes) <= 30);

            // Find flights 7 days after within +/-30 minutes
            bool hasNextFlight = flights.Any(f =>
                f.AirlineId == flight.AirlineId &&
                f.Route.OriginCityId == flight.Route.OriginCityId &&
                f.Route.DestinationCityId == flight.Route.DestinationCityId &&
                Math.Abs((f.DepartureTime - nextWeekTarget).TotalMinutes) <= 30);

            // New Flight
            if (!hasPreviousFlight)
            {
                results.Add(new ChangeResult
                {
                    FlightId = flight.FlightId,
                    OriginCityId = flight.Route.OriginCityId,
                    DestinationCityId = flight.Route.DestinationCityId,
                    DepartureTime = flight.DepartureTime,
                    ArrivalTime = flight.ArrivalTime,
                    AirlineId = flight.AirlineId,
                    Status = "New"
                });
            }

            // Discontinued Flight
            if (!hasNextFlight)
            {
                results.Add(new ChangeResult
                {
                    FlightId = flight.FlightId,
                    OriginCityId = flight.Route.OriginCityId,
                    DestinationCityId = flight.Route.DestinationCityId,
                    DepartureTime = flight.DepartureTime,
                    ArrivalTime = flight.ArrivalTime,
                    AirlineId = flight.AirlineId,
                    Status = "Discontinued"
                });
            }
        }

        // Export results to CSV
        var outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "results.csv");

        using var writer = new StreamWriter(outputFilePath);
        using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csvWriter.WriteRecords(results);

        Console.WriteLine($"Results exported successfully to {outputFilePath}");

        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        stopwatch.Stop();
        Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");

    }
}
