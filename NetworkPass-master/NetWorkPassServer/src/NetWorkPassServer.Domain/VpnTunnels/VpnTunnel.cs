using Abp.Domain.Entities.Auditing;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Devices;
using NetWorkPassServer.Domain.Shared;

namespace NetWorkPassServer.Domain.VpnTunnels;

public sealed class VpnTunnel : FullAuditedAggregateRoot<Guid>
{
    private VpnTunnel()
    {
    }

    public VpnTunnel(
        Guid branchId,
        string tunnelName,
        IpAddress remoteIpAddress,
        bool isCritical)
    {
        if (branchId == Guid.Empty)
        {
            throw new ArgumentException(
                "BranchId boş ola bilməz");
        }

        if (string.IsNullOrWhiteSpace(tunnelName))
        {
            throw new ArgumentException(
                "Tunnel adı boş ola bilməz");
        }

        BranchId = branchId;

        TunnelName = tunnelName.Trim();

        RemoteIpAddress = remoteIpAddress;

        IsCritical = isCritical;

        Status = VpnStatus.Unknown;

        IsMonitoringEnabled = true;

        IsActive = true;

        IsInMaintenanceMode = false;
    }

    // RELATIONS

    public Guid BranchId { get; private set; }

    public Branch Branch { get; private set; } = default!;

    // VPN INFO

    public string TunnelName { get; private set; } = default!;

    public IpAddress RemoteIpAddress { get; private set; } = default!;

    // STATUS

    public VpnStatus Status { get; private set; }

    public bool IsMonitoringEnabled { get; private set; }

    public bool IsInMaintenanceMode { get; private set; }

    public bool IsActive { get; private set; }

    public bool IsCritical { get; private set; }

    // METRICS

    public long? PingLatency { get; private set; }

    public DateTime? LastSeenAt { get; private set; }

    public DateTime? LastConnectedAt { get; private set; }

    public DateTime? LastDisconnectedAt { get; private set; }

    public int ConsecutiveFailureCount { get; private set; }

    public int ConsecutiveSuccessCount { get; private set; }

    // LIFECYCLE

    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;

        IsMonitoringEnabled = true;

        Status = VpnStatus.Unknown;
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;

        IsMonitoringEnabled = false;

        Status = VpnStatus.Unknown;
    }

    // MONITORING

    public void EnableMonitoring()
    {
        if (IsMonitoringEnabled)
        {
            return;
        }

        IsMonitoringEnabled = true;
    }

    public void DisableMonitoring()
    {
        if (!IsMonitoringEnabled)
        {
            return;
        }

        IsMonitoringEnabled = false;
    }

    // MAINTENANCE

    public void EnableMaintenanceMode()
    {
        if (IsInMaintenanceMode)
        {
            return;
        }

        IsInMaintenanceMode = true;

        Status = VpnStatus.Maintenance;
    }

    public void DisableMaintenanceMode()
    {
        if (!IsInMaintenanceMode)
        {
            return;
        }

        IsInMaintenanceMode = false;

        Status = VpnStatus.Unknown;
    }

    // RUNTIME

    public void MarkConnected(
        long? latency)
    {
        if (!IsActive)
        {
            return;
        }

        if (!IsMonitoringEnabled)
        {
            return;
        }

        if (IsInMaintenanceMode)
        {
            return;
        }

        ConsecutiveFailureCount = 0;

        ConsecutiveSuccessCount++;

        PingLatency = latency;

        LastSeenAt = DateTime.UtcNow;

        LastConnectedAt = DateTime.UtcNow;

        if (ConsecutiveSuccessCount < 3)
        {
            return;
        }

        if (latency.HasValue &&
            latency.Value > 500)
        {
            Status = VpnStatus.Degraded;

            return;
        }

        Status = VpnStatus.Connected;
    }

    public void MarkDisconnected()
    {
        if (!IsActive)
        {
            return;
        }

        if (!IsMonitoringEnabled)
        {
            return;
        }

        if (IsInMaintenanceMode)
        {
            return;
        }

        ConsecutiveSuccessCount = 0;

        ConsecutiveFailureCount++;

        LastDisconnectedAt = DateTime.UtcNow;

        if (ConsecutiveFailureCount < 3)
        {
            return;
        }

        Status = VpnStatus.Disconnected;
    }

    // CONFIGURATION

    public void ChangeTunnelName(
        string tunnelName)
    {
        if (string.IsNullOrWhiteSpace(tunnelName))
        {
            throw new ArgumentException(
                "Tunnel adı boş ola bilməz");
        }

        TunnelName = tunnelName.Trim();
    }

    public void ChangeRemoteIp(
        IpAddress remoteIpAddress)
    {
        RemoteIpAddress = remoteIpAddress;
    }

    public void SetCritical(
        bool isCritical)
    {
        IsCritical = isCritical;
    }
}