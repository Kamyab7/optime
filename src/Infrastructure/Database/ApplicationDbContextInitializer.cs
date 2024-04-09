﻿using Bogus;
using Domain;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Infrastructure.Database;

public class ApplicationDbContextInitializer
{
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IRecurringJobManager _recurringJobManager;

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
           .CustomInstantiator(f => new Mission(new Point(f.Random.Double(-90, 90), f.Random.Double(-180, 180)),
                                                new Point(f.Random.Double(-90, 90), f.Random.Double(-180, 180))));

        // Generate 20 instances of mock data
        List<Mission> missions = missionFaker.Generate(20);

        await _context.Missions.AddRangeAsync(missions);

        await _context.SaveChangesAsync();
    }

    public void AddMockMissionDataCronJob()=> _recurringJobManager.AddOrUpdate("MissionMockData", () => AddMockMissionData(), "*/30 * * * * *");
}
