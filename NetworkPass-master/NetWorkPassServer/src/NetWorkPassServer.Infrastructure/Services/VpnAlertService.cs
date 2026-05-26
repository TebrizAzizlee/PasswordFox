using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Services;
using NetWorkPassServer.Domain.Alerts;
using NetWorkPassServer.Domain.VpnTunnels;

namespace NetWorkPassServer.Infrastructure.Services;
internal sealed class VpnAlertService(
    IPasswordDbContext context)
    : IVpnAlertService
{
    public async Task HandleVpnStateChangedAsync(
        VpnTunnel vpnTunnel,
        CancellationToken cancellationToken)
    {
        if (vpnTunnel.IsInMaintenanceMode)
        {
            return;
        }

        var fingerprint =
            $"vpn:{vpnTunnel.Id}:{vpnTunnel.Status}";

        var existingAlert =
            await context.Alerts
                .FirstOrDefaultAsync(
                    x =>
                        x.Fingerprint == fingerprint &&
                        x.Status!=AlertStatus.Resolved,
                    cancellationToken);

        if (existingAlert is not null)
        {
            return;
        }

        Alert? alert = null;

        if (vpnTunnel.Status ==
            VpnStatus.Disconnected)
        {
            alert = new Alert(
                Guid.Empty,
                vpnTunnel.BranchId,
                AlertType.VpnDisconnected,
                AlertSeverity.Critical,
                AlertSource.Vpn,
                "VPN bağlantısı kəsildi",
                $"{vpnTunnel.TunnelName} VPN bağlantısı kəsildi",
                DateTime.Now,
                fingerprint);
        }

        if (vpnTunnel.Status ==
            VpnStatus.Degraded)
        {
            alert = new Alert(
                Guid.Empty,
                vpnTunnel.BranchId,
                AlertType.VpnDegraded,
                AlertSeverity.Warning,
                AlertSource.Vpn,
                "VPN gecikməsi yüksəkdir",
                $"{vpnTunnel.TunnelName} latency yüksəkdir",
                DateTime.Now,
                fingerprint);
        }

        if (alert is null)
        {
            return;
        }

        await context.Alerts.AddAsync(
            alert,
            cancellationToken);

        await context.SaveChangesAsync(
            cancellationToken);
    }
}