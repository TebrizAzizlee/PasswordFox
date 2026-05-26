using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Domain.Alerts;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.DeviceMetricss;
using NetWorkPassServer.Domain.Devices;
using NetWorkPassServer.Domain.VpnTunnelHeartbeats;
using NetWorkPassServer.Domain.VpnTunnels;


namespace NetWorkPassServer.Application.Context;
public interface IPasswordDbContext
{
    DbSet<Branch> Branches { get; }

    DbSet<Device> Devices { get; }

    DbSet<Alert> Alerts { get; }
    DbSet<DeviceMetric> DeviceMetrics { get; }
    DbSet<VpnTunnel> VpnTunnels { get; }
    DbSet<VpnTunnelHeartbeat> VpnTunnelHeartbeats { get; }
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}
