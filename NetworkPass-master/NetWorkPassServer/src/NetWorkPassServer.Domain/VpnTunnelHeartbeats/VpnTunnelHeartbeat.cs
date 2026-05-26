using Abp.Domain.Entities.Auditing;
using NetWorkPassServer.Domain.VpnTunnels;

namespace NetWorkPassServer.Domain.VpnTunnelHeartbeats;

public sealed class VpnTunnelHeartbeat
    : FullAuditedAggregateRoot<Guid>
{
    private VpnTunnelHeartbeat()
    {
    }

    public VpnTunnelHeartbeat(
        Guid vpnTunnelId,
        VpnStatus status,
        bool isReachable,
        long? latency,
        string? errorMessage)
    {
        VpnTunnelId = vpnTunnelId;

        Status = status;

        IsReachable = isReachable;

        Latency = latency;

        ErrorMessage = errorMessage;

        ReceivedAt = DateTime.UtcNow;
    }

    public Guid VpnTunnelId { get; private set; }

    public VpnTunnel VpnTunnel { get; private set; } = default!;

    public VpnStatus Status { get; private set; }

    public bool IsReachable { get; private set; }

    public long? Latency { get; private set; }

    public string? ErrorMessage { get; private set; }

    public DateTime ReceivedAt { get; private set; }
}