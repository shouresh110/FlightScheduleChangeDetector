using FlightScheduleChangeDetector.Import;
using FlightScheduleChangeDetector.Data;
using FlightScheduleChangeDetector.Services;

namespace FlightScheduleChangeDetector;

internal class Program
{
    static void Main(string[] args)
    {
        using var dbContext = new FlightScheduleDbContext();

        dbContext.Database.EnsureCreated();

        string csvFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "DataFiles");

        var importer = new CsvImporter(dbContext);
        importer.ImportAll(csvFolderPath);

        Console.WriteLine("✅ Data imported successfully.");

        // ✅ validate command line args
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: FlightScheduleChangeDetector <startDate:yyyy-MM-dd> <endDate:yyyy-MM-dd> <agencyId:int>");
            return;
        }

        if (!DateTime.TryParse(args[0], out var startDate) ||
            !DateTime.TryParse(args[1], out var endDate) ||
            !int.TryParse(args[2], out var agencyId))
        {
            Console.WriteLine("❌ Invalid arguments. Please use: yyyy-MM-dd yyyy-MM-dd <agencyId>");
            return;
        }

        // ✅ run change detection
        var detector = new ChangeDetectionService(dbContext);
        detector.DetectChanges(startDate, endDate, agencyId);
    }
}
