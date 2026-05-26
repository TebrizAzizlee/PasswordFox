using Microsoft.AspNetCore.SignalR;
using NetWorkPassServer.Application.Notifications;
using NetWorkPassServer.Domain.VpnTunnels;
using NetWorkPassServer.Infrastructure.Hubs;


namespace NetWorkPassServer.Infrastructure.Notifications;
internal sealed class VpnRealtimeNotifier(
    IHubContext<MonitoringHub> hubContext)
    : IVpnRealtimeNotifier
{
    public async Task TunnelStatusChangedAsync(
        Guid tunnelId,
        Guid branchId,
        string tunnelName,
        VpnStatus status,
        long? latency,
        DateTime? lastSeenAt,
        CancellationToken cancellationToken)
    {
        await hubContext.Clients
            .Group($"branch-{branchId}")
            .SendAsync(
                "vpn-status-updated",
                new
                {
                    TunnelId = tunnelId,
                    BranchId = branchId,
                    TunnelName = tunnelName,
                    Status = status.ToString(),
                    Latency = latency,
                    LastSeenAt = lastSeenAt
                },
                cancellationToken);
    }
}