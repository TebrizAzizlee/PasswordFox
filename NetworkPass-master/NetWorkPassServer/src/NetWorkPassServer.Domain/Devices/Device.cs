using Abp.Domain.Entities.Auditing;
using NetWorkPassServer.Domain.Alerts;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.DeviceHeartbeats;
using NetWorkPassServer.Domain.DeviceMetricss;
using NetWorkPassServer.Domain.Shared;

namespace NetWorkPassServer.Domain.Devices;
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
        string vendor,
        DeviceRole role,
        string model,
         bool iscritical,
        string? description
        )
    {
        BranchId = branchId;
        Name = name;
        IpAddress = ipAddress;
        Type = type;
        Vendor = vendor;
        Role = role;
        IsCritical=iscritical;
        Description = description;
        Model = model;
        Metrics = [];
        Heartbeats = [];
        Alerts = [];

        Status = DeviceStatus.Unknown;

        IsMonitoringEnabled = true;
        IsActive=true;
        ConsecutiveFailureCount = 0;
    }

    public Guid BranchId { get; private set; } = default;

    public DeviceName Name { get; private set; } = default!;

    public IpAddress IpAddress { get; private set; } = default!;

    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = default;
    public string Model { get; private set; } = default!;

    public string Vendor { get; private set; } = default!;

    public DeviceType Type { get; private set; }

    public DeviceRole Role { get; private set; }

    public bool IsCritical { get; private set; } = default!;
    public int ConsecutiveFailureCount { get; private set; }
    public DeviceStatus Status { get; private set; }

    public DateTime? LastSeenAt { get; private set; }
    public DateTime? LastHeartbeatAttemptAt { get; private set; }
    public DateTime? LastStatusChangeAt { get; private set; }
    public long UptimeSeconds { get; private set; } = default!;

    public bool IsMonitoringEnabled { get; private set; } = default!;

    public double? CpuUsage { get; private set; }

    public double? MemoryUsage { get; private set; }

    public double? Temperature { get; private set; }

    public long? PingLatency { get; private set; }

    public Branch Branch { get; private set; } = default!;

    public ICollection<DeviceMetric> Metrics { get; private set; } = default!;

    public ICollection<DeviceHeartbeat> Heartbeats { get; private set; } = default!;

    public ICollection<Alert> Alerts { get; private set; } = default!;

    public void Update(
        DeviceName name,
        IpAddress ipAddress,
        DeviceType type,
        string vendor,
        DeviceRole role,
        string model,
        bool isCritical,
        string? description)
    {
        Name = name;
        IpAddress = ipAddress;
        Type = type;
        Vendor = vendor;
        Role = role;
        Model = model;
        IsCritical = isCritical;
        Description = description;
    }
    public void MarkAsDeleted()
    {
        if (IsDeleted)
        {
            return;
        }

        IsDeleted = true;

        IsActive = false;

        DisableMonitoring();

        Status = DeviceStatus.Unknown;
    }
    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;

        DisableMonitoring();
        Status = DeviceStatus.Unknown;
    }
    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;

    }
    public void EnableMonitoring()
    {
        if (IsMonitoringEnabled)
        {
            return;
        }

        IsMonitoringEnabled = true;
        ConsecutiveFailureCount = 0;

        Status = DeviceStatus.Unknown;
    }

    public void DisableMonitoring()
    {
        if (!IsMonitoringEnabled)
        {
            return;
        }

        IsMonitoringEnabled = false;
    }

    public void MarkHeartbeatSuccess(
     long? responseTimeMs, DateTime utcNow)
    {
        ConsecutiveFailureCount = 0;
        LastHeartbeatAttemptAt = utcNow;
        LastSeenAt = utcNow;

        PingLatency = responseTimeMs;
        EvaluateHealthStatus(
            utcNow);
    }
    public void MarkHeartbeatFailure(
        DateTime utcNow)
    {
        LastHeartbeatAttemptAt = utcNow;

        ConsecutiveFailureCount++;

        

        if (ConsecutiveFailureCount < 3)
        {
            return;
        }

        ChangeStatus(
            DeviceStatus.Offline,
            utcNow);
    }
    public void UpdateMetrics(
        double? cpuUsage,
        double? memoryUsage,
        double? temperature,
        long uptimeSeconds)
    {
        CpuUsage = cpuUsage;

        MemoryUsage = memoryUsage;

        Temperature = temperature;

        UptimeSeconds = uptimeSeconds;
    }
    public void EvaluateHealthStatus(
       DateTime utcNow)
    {
        // 🔥 offline state handled separately

        if (ConsecutiveFailureCount >= 3)
        {
            ChangeStatus(
                DeviceStatus.Offline,
                utcNow);

            return;
        }

        // 🔥 degraded checks

        var highLatency =
            PingLatency.HasValue &&
            PingLatency.Value > 500;

        var highCpu =
            CpuUsage.HasValue &&
            CpuUsage.Value >= 90;

        var highMemory =
            MemoryUsage.HasValue &&
            MemoryUsage.Value >= 90;

        var highTemperature =
            Temperature.HasValue &&
            Temperature.Value >= 80;

        if (highLatency ||
            highCpu ||
            highMemory ||
            highTemperature)
        {
            ChangeStatus(
                DeviceStatus.Degraded,
                utcNow);

            return;
        }
        ChangeStatus(
         DeviceStatus.Online,
         utcNow);

    }
    private void ChangeStatus(
        DeviceStatus newStatus,
        DateTime utcNow)
    {
        if (Status == newStatus)
        {
            return;
        }

        Status = newStatus;

        LastStatusChangeAt = utcNow;
    }
}
