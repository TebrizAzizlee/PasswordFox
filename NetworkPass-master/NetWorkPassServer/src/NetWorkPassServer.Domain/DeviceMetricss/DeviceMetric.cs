using Abp.Domain.Entities;


namespace NetWorkPassServer.Domain.DeviceMetricss;
public sealed class DeviceMetric : Entity<Guid>
{
    private DeviceMetric()
    {

    }
    public Guid DeviceId { get; private set; }

    public DateTime Timestamp { get; private set; }

    public double? CpuUsage { get; private set; }

    public double MemoryUsage { get; private set; }

    public double DiskUsage { get; private set; }
    public long PingLatency { get;private set; }
    public double Temperature { get; private set; }
    public Device Device { get; private set; } = default!;
    public DeviceMetric(
          Guid deviceId,
          DateTime timestamp,
          double cpuUsage,
          double memoryUsage,
          double diskUsage,
          double temperature,long pingLatency)
    {
        DeviceId = deviceId;
        Timestamp = timestamp;

        CpuUsage = cpuUsage;
        MemoryUsage = memoryUsage;
        DiskUsage = diskUsage;
        Temperature = temperature;
        PingLatency = pingLatency;
    }
}
