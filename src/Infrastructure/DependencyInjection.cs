using Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Application.Common.Interfaces;
using Hangfire;
using Infrastructure.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        ArgumentNullException.ThrowIfNullOrEmpty(connectionString, nameof(connectionString));

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, x => x.UseNetTopologySuite());
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitializer>();

        services.AddScoped<AutoAssignerService>();

        services.AddHangfire(configuration => configuration
           .UseInMemoryStorage());

        services.AddHangfireServer();

        services.AddScoped<IBus, FakeBus>();

        return services;
    }
}