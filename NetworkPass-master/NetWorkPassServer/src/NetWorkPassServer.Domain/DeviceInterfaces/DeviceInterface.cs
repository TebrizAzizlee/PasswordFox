using Abp.Domain.Entities;
using NetWorkPassServer.Domain.Devices;



namespace NetWorkPassServer.Domain.DeviceInterfaces;
public sealed class DeviceInterface : Entity<Guid>
{
    private DeviceInterface()
    {
    }

    public DeviceInterface(
        Guid deviceId,
        string name)
    {
        DeviceId = deviceId;
        Name = name;
    }

    public Guid DeviceId { get; private set; }

    public string Name { get; private set; } = default!;

    public IpAddress? IpAddress { get; private set; }

    public string? MacAddress { get; private set; }

    public InterfaceStatus Status { get; private set; }

    public long RxBytes { get; private set; }

    public long TxBytes { get; private set; }

    public long RxErrors { get; private set; }

    public long TxErrors { get; private set; }

    public long RxDroppedPackets { get; private set; }

    public long TxDroppedPackets { get; private set; }

    public int? SpeedMbps { get; private set; }

    public DateTime? LastSeenAt { get; private set; }

    public Device Device { get; private set; } = default!;
}
