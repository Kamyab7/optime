using Bogus;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Database;

public class ApplicationDbContextInitializer
{
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
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
}
