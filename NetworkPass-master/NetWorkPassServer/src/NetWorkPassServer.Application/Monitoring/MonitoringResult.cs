using NetWorkPassServer.Domain.Devices;


namespace NetWorkPassServer.Application.Monitoring;
public sealed class MonitoringResult
{
    public bool IsReachable { get; init; }

    public long? ResponseTimeMs { get; init; }
    public DateTime OccurredAtUtc { get; init; }
    public string? ErrorMessage { get; init; }
    public double? DiskUsage {  get; init; }
    public DeviceStatus Status { get; init; }

    // 🔥 metrics

    public double? CpuUsage { get; init; }

    public double? MemoryUsage { get; init; }

    public double? Temperature { get; init; }

    public long? UptimeSeconds { get; init; }
}
