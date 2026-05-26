using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using NetWorkPassServer.Domain.Devices;

namespace NetWorkPassServer.Domain.DeviceMetricss;

public sealed class DeviceMetric : Entity<Guid>
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
        long? uptimeSeconds,
        long? pingLatency)
    {
        DeviceId = deviceId;

        OccurredAtUtc = occurredAtUtc;

        CpuUsage = cpuUsage;
        MemoryUsage = memoryUsage;
        DiskUsage = diskUsage;

        Temperature = temperature;
        UptimeSeconds = uptimeSeconds;
        PingLatency = pingLatency;
    }

    public Guid DeviceId { get; private set; }

    public DateTime OccurredAtUtc { get; private set; }

    public double? CpuUsage { get; private set; }

    public double? MemoryUsage { get; private set; }

    public double? DiskUsage { get; private set; }

    public double? Temperature { get; private set; }
    public long? UptimeSeconds { get; private set; }
    public long? PingLatency { get; private set; }

    public Device Device { get; private set; } = default!;

    public static DeviceMetric Create(
        Guid deviceId,
        DateTime occurredAtUtc,
        double? cpuUsage,
        double? memoryUsage,
        double? diskUsage,
        double? temperature,
        long ? uptimeSeconds,
        long? pingLatency)
    {
        if (deviceId == Guid.Empty)
        {
            throw new ArgumentException(
                "DeviceId boş ola bilməz");
        }
        // 🔥 utc normalize
        if (occurredAtUtc.Kind != DateTimeKind.Utc)
        {
            occurredAtUtc =
                DateTime.SpecifyKind(
                    occurredAtUtc,
                    DateTimeKind.Utc);
        }
        // 🔥 validation

        ValidatePercentage(
            cpuUsage,
            nameof(cpuUsage));

        ValidatePercentage(
            memoryUsage,
            nameof(memoryUsage));

        ValidatePercentage(
            diskUsage,
            nameof(diskUsage));

        ValidateTemperature(
            temperature);

        ValidateUptime(
            uptimeSeconds);

        ValidateLatency(
            pingLatency);

        return new DeviceMetric(
            deviceId,
            occurredAtUtc,
            cpuUsage,
            memoryUsage,
            diskUsage,
            temperature,
            uptimeSeconds,
            pingLatency);
    }

    private static void ValidatePercentage(
       double? value,
       string fieldName)
    {
        if (!value.HasValue)
        {
            return;
        }

        if (value < 0 || value > 100)
        {
            throw new ArgumentException(
                $"{fieldName} 0-100 arasında olmalıdır");
        }
    }

    private static void ValidateTemperature(
        double? temperature)
    {
        if (!temperature.HasValue)
        {
            return;
        }

        // 🔥 realistic hardware range

        if (temperature < -50 ||
            temperature > 150)
        {
            throw new ArgumentException(
                "Temperature düzgün deyil");
        }
    }

    private static void ValidateUptime(
        long? uptimeSeconds)
    {
        if (!uptimeSeconds.HasValue)
        {
            return;
        }

        if (uptimeSeconds < 0)
        {
            throw new ArgumentException(
                "Uptime mənfi ola bilməz");
        }
    }

    private static void ValidateLatency(
        long? pingLatency)
    {
        if (!pingLatency.HasValue)
        {
            return;
        }

        if (pingLatency < 0)
        {
            throw new ArgumentException(
                "Ping latency mənfi ola bilməz");
        }
    }
}