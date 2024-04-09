using Infrastructure.Services;

namespace WebAPI.Extensions;

public static class AutoAssignerExtensions
{
    public static async Task RunAutoAssigner(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<AutoAssignerService>();

        initializer.AutoAssignCronJob();
    }
}
