using Abp.Domain.Entities.Auditing;

namespace NetWorkPassServer.Domain.DeviceMetricss;

public sealed class DeviceMetric : CreationAuditedEntity<Guid>
{
    private DeviceMetric()
    {
    }

    private DeviceMetric(
        Guid deviceId,
        DateTime occurredAtUtc,
        double? cpuUsage,
        double? memoryUsage,
        double? diskUsage,
        double? temperature,
        long? pingLatency)
    {
        DeviceId = deviceId;

        OccurredAtUtc = occurredAtUtc;

        CpuUsage = cpuUsage;
        MemoryUsage = memoryUsage;
        DiskUsage = diskUsage;

        Temperature = temperature;

        PingLatency = pingLatency;
    }

    public Guid DeviceId { get; private set; }

    public DateTime OccurredAtUtc { get; private set; }

    public double? CpuUsage { get; private set; }

    public double? MemoryUsage { get; private set; }

    public double? DiskUsage { get; private set; }

    public double? Temperature { get; private set; }

    public long? PingLatency { get; private set; }

    public Device Device { get; private set; } = default!;

    public static DeviceMetric Create(
        Guid deviceId,
        DateTime occurredAtUtc,
        double? cpuUsage,
        double? memoryUsage,
        double? diskUsage,
        double? temperature,
        long? pingLatency)
    {
        if (occurredAtUtc.Kind != DateTimeKind.Utc)
        {
            occurredAtUtc =
                DateTime.SpecifyKind(
                    occurredAtUtc,
                    DateTimeKind.Utc);
        }

        return new DeviceMetric(
            deviceId,
            occurredAtUtc,
            cpuUsage,
            memoryUsage,
            diskUsage,
            temperature,
            pingLatency);
    }
}