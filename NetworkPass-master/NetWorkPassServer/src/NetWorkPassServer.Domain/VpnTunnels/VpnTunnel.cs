using Abp.Domain.Entities;
using NetWorkPassServer.Domain.Devices;
using System.Net.NetworkInformation;

namespace NetWorkPassServer.Domain.VpnTunnels;

public sealed class VpnTunnel : Entity<Guid>
{
    private VpnTunnel()
    {
    }

    public VpnTunnel(
        Guid branchId,
        string tunnelName,
        IpAddress remoteIpAddress)
    {
        BranchId = branchId;

        TunnelName = tunnelName;

        RemoteIpAddress = remoteIpAddress;

        Status = VpnStatus.Unknown;

        IsMonitoringEnabled = true;
    }

    public Guid BranchId { get; private set; }

    public string TunnelName { get; private set; } = default!;

    public IpAddress RemoteIpAddress { get; private set; } = default!;

    public VpnStatus Status { get; private set; }

    public DateTime? LastConnectedAt { get; private set; }

    public DateTime? LastDisconnectedAt { get; private set; }

    public long? PingLatency { get; private set; }

    public bool IsMonitoringEnabled { get; private set; }

    public bool IsCritical { get; private set; }

    public Branch Branch { get; private set; } = default!;

    public void MarkConnected(
        long? latency)
    {
        Status = VpnStatus.Connected;

        PingLatency = latency;

        LastConnectedAt = DateTime.UtcNow;
    }

    public void MarkDisconnected()
    {
        Status = VpnStatus.Disconnected;

        LastDisconnectedAt = DateTime.UtcNow;
    }
}