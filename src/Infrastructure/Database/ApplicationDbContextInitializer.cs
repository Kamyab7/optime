using Bogus;
using Domain;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using System;

namespace Infrastructure.Database;

public class ApplicationDbContextInitializer
{
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly Random _random = Random.Shared;

    public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger, ApplicationDbContext context, IRecurringJobManager recurringJobManager)
    {
        _logger = logger;
        _context = context;
        _recurringJobManager = recurringJobManager;
    }

    public async Task InitializeAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        if (_context.Drivers.Any())
            return;

        var driverFaker = new Faker<Driver>()
                    .CustomInstantiator(f => new Driver(f.Person.FirstName, f.Person.LastName))
                    .RuleFor(d => d.Missions, f => null); // Missions will be null for mock data

        // Generate 75 instances of mock data
        List<Driver> drivers = driverFaker.Generate(75);

        _context.Drivers.AddRange(drivers);

        await _context.SaveChangesAsync();
    }

    public async Task AddMockMissionData()
    {
        var missionFaker = new Faker<Mission>()
           .CustomInstantiator(f =>
           {
               // Generate latitude and longitude within valid ranges
               double sourceLatitude = f.Random.Double(-90, 90);
               double sourceLongitude = f.Random.Double(-180, 180);
               double destinationLatitude = f.Random.Double(-90, 90);
               double destinationLongitude = f.Random.Double(-180, 180);

               // Create valid geometry points with SRID 4326
               var sourcePoint = new Point(sourceLongitude, sourceLatitude) { SRID = 4326 };
               var destinationPoint = new Point(destinationLongitude, destinationLatitude) { SRID = 4326 };

               return new Mission(destinationPoint, sourcePoint);
           });

        // Generate 20 instances of mock data
        List<Mission> missions = missionFaker.Generate(20);

        await _context.Missions.AddRangeAsync(missions);

        await _context.SaveChangesAsync();

        _logger.LogInformation("20 Mock mission data added.");
    }

    public async Task AddMockArrivedData()
    {
        var randomDriver = _context.Drivers.Include(d => d.Missions)
            .Where(d => d.Missions!.Any(m => m.MissionStatus == MissionStatus.InProgress))
            .OrderBy(x => _random.Next())
            .FirstOrDefault();

        if (randomDriver == null)
            return;

        var mission = randomDriver.Missions!
            .Where(m => m.MissionStatus == MissionStatus.InProgress)
            .FirstOrDefault();

        if (mission == null) 
            return;

        mission.CompleteMission();

        await _context.SaveChangesAsync();
    }

    public void AddMockMissionDataCronJob() => _recurringJobManager.AddOrUpdate("MissionMockData", () => AddMockMissionData(), "*/20 * * * * *");

    public void AddMockDriverArrivedCronJob() => _recurringJobManager.AddOrUpdate("ArrivedMockData", () => AddMockArrivedData(), "*/2 * * * * *");
}
