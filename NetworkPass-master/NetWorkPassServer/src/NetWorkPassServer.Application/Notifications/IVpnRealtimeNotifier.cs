using NetWorkPassServer.Domain.VpnTunnels;


namespace NetWorkPassServer.Application.Notifications;
public interface IVpnRealtimeNotifier
{
    Task TunnelStatusChangedAsync(
        Guid tunnelId,
        Guid branchId,
        string tunnelName,
        VpnStatus status,
        long? latency,
        DateTime? lastSeenAt,
        CancellationToken cancellationToken);
}