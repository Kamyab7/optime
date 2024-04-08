using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Driver> Drivers { get; }

    DbSet<Mission> Missions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
