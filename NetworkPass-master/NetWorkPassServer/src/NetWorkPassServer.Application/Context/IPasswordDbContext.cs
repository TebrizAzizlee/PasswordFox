using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Domain.Alerts;
using NetWorkPassServer.Domain.DeviceMetricss;


namespace NetWorkPassServer.Application.Context;
public interface IPasswordDbContext
{
    DbSet<Branch> Branches { get; }

    DbSet<Device> Devices { get; }

    DbSet<Alert> Alerts { get; }
    DbSet<DeviceMetric> DeviceMetrics { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}
