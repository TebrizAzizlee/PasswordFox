using Abp.Domain.Entities;
using NetWorkPassServer.Domain.Devices;
using NetWorkPassServer.Domain.InterfaceMetrics;
using NetWorkPassServer.Domain.Shared;



namespace NetWorkPassServer.Domain.DeviceInterfaces;
public sealed class DeviceInterface
{
    private DeviceInterface()
    {
    }

    public DeviceInterface(
        Guid deviceId,
        string name,
        InterfaceType type)
    {
        DeviceId = deviceId;

        Name = name;

        Type = type;

        Status = InterfaceStatus.Unknown;

        IsMonitoringEnabled = true;
    }

    public Guid Id { get; private set; }

    public Guid DeviceId { get; private set; }

    public string Name { get; private set; } = default!;

    public string? Alias { get; private set; }

    public InterfaceType Type { get; private set; }

    public InterfaceStatus Status { get; private set; }

    public long? SpeedMbps { get; private set; }

    public bool IsUplink { get; private set; }

    public bool IsMonitoringEnabled { get; private set; }

    public DateTime? LastSeenAt { get; private set; }

    public Device Device { get; private set; } = default!;

    public ICollection<InterfaceMetric> Metrics { get; private set; }
        = new List<InterfaceMetric>();

    public void UpdateStatus(
        InterfaceStatus status)
    {
        Status = status;

        LastSeenAt = DateTime.UtcNow;
    }

    public void SetAlias(
        string? alias)
    {
        Alias = alias;
    }

    public void SetSpeed(
        long? speedMbps)
    {
        SpeedMbps = speedMbps;
    }

    public void MarkAsUplink()
    {
        IsUplink = true;
    }

    public void RemoveUplink()
    {
        IsUplink = false;
    }

    public void EnableMonitoring()
    {
        IsMonitoringEnabled = true;
    }

    public void DisableMonitoring()
    {
        IsMonitoringEnabled = false;
    }
}