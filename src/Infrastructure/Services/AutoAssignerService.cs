using Application.Common.Interfaces;
using Domain;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class AutoAssignerService
{
    private readonly ILogger<AutoAssignerService> _logger;

    private readonly IRecurringJobManager _recurringJobManager;

    private readonly IApplicationDbContext _context;

    private readonly IBus _bus;

    private readonly CancellationTokenSource cancellationTokenSource;

    private readonly CancellationToken cancellationToken;

    public AutoAssignerService(ILogger<AutoAssignerService> logger
        , IRecurringJobManager recurringJobManager
        , IApplicationDbContext context
        , IBus bus)
    {
        _logger = logger;
        _recurringJobManager = recurringJobManager;
        _context = context;
        _bus = bus;
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;
    }

    public async Task AssignMission(CancellationToken cancellationToken)
    {
        var availableDrivers =  await _context.Drivers
            .Include(d => d.Missions)
            .Where(d => !d.Missions!.Any(m => m.MissionStatus == MissionStatus.InProgress))
            .ToListAsync();

        var availableMissions = await _context.Missions
            .Where(m => m.MissionStatus == MissionStatus.Pending)
            .ToListAsync();

        foreach (var driver in availableDrivers)
        {
            var driverCurrentLocation = _bus.Publish(driver.Id);

            var suitableMission = availableMissions.OrderBy(m => m.Source.Distance(driverCurrentLocation))
                .ThenBy(m => m.Destination.Distance(driverCurrentLocation)).FirstOrDefault();

            if (suitableMission == null)
                return;

            suitableMission.AssignToDriver(driver);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"mission: {suitableMission.Id} assigned to driver: {driver.Id}");
        }
    }

    public void AutoAssignCronJob() => _recurringJobManager.AddOrUpdate("MissionAssigner", () => AssignMission(cancellationToken), "*/5 * * * * *");
}