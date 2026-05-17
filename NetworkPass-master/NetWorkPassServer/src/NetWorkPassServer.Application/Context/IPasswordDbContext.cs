using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Domain.Alerts;


namespace NetWorkPassServer.Application.Context;
public interface IPasswordDbContext
{
    DbSet<Branch> Branches { get; }

    DbSet<Device> Devices { get; }

    DbSet<Alert> Alerts { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}
