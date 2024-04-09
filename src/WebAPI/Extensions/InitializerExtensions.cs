using Infrastructure.Database;

namespace WebAPI.Extensions;

public static class InitializerExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

        await initializer.InitializeAsync();

        await initializer.SeedAsync();

        initializer.AddMockMissionDataCronJob();

        initializer.AddMockDriverArrivedCronJob();
    }
}
