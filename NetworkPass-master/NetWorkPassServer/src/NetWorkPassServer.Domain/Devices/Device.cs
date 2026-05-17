using Abp.Domain.Entities.Auditing;
using NetWorkPassServer.Domain.Alerts;
using NetWorkPassServer.Domain.DeviceHeartbeats;
using NetWorkPassServer.Domain.DeviceMetrics;
using NetWorkPassServer.Domain.Devices;



public sealed class Device : FullAuditedAggregateRoot<Guid>
{
    private Device()
    {
    }

    public Device(
        Guid branchId,
        DeviceName name,
        IpAddress ipAddress,
        DeviceType type,
        string? description)
    {
        BranchId = branchId;
        Name = name;
        IpAddress = ipAddress;
        Type = type;
        Description = description;

        Metrics = new List<DeviceMetric>();
        Heartbeats = new List<DeviceHeartbeat>();
        Alerts = new List<Alert>();

        Status = DeviceStatus.Unknown;

        IsMonitoringEnabled = true;
       
    }

    public Guid BranchId { get; private set; } = default;

    public DeviceName Name { get; private set; } = default!;

    public IpAddress IpAddress { get; private set; } = default!;

    public string? Description { get; private set; }

    public string Model { get; private set; } = default!;

    public DeviceVendor Vendor { get; private set; }= default!;

    public DeviceType Type { get; private set; } = default!;

    public DeviceRole Role { get; private set; } = default!;

    public bool IsCritical { get; private set; } = default!;
    public int ConsecutiveFailureCount { get; private set; }
    public DeviceStatus Status { get; private set; } = default!;

    public DateTime? LastSeenAt { get; private set; } = default!;

    public long UptimeSeconds { get; private set; } = default!;

    public bool IsMonitoringEnabled { get; private set; } = default!;

    public double? CpuUsage { get; private set; } = default!;

    public double? MemoryUsage { get; private set; } = default!;

    public double? Temperature { get; private set; } = default!;

    public double? PingLatency { get; private set; } = default!;

    public Branch Branch { get; private set; } = default!;

    public ICollection<DeviceMetric> Metrics { get; private set; } = default!;

    public ICollection<DeviceHeartbeat> Heartbeats { get; private set; } = default!;

    public ICollection<Alert> Alerts { get; private set; } = default!;

    public void Update(
        DeviceName name,
        IpAddress ipAddress,
        DeviceType type,
        string? description)
    {
        Name = name;
        IpAddress = ipAddress;
        Type = type;
        Description = description;
    }

    public void MarkHeartbeatSuccess(
     int? responseTimeMs)
    {
        ConsecutiveFailureCount = 0;

        LastSeenAt = DateTime.UtcNow;

        PingLatency = responseTimeMs;

        if (responseTimeMs.HasValue &&
            responseTimeMs.Value > 500)
        {
            Status = DeviceStatus.Warning;

            return;
        }

        Status = DeviceStatus.Online;
    }
    public void MarkHeartbeatFailure()
    {
        ConsecutiveFailureCount++;

        if (ConsecutiveFailureCount >= 3)
        {
            Status = DeviceStatus.Offline;
        }
    }
}