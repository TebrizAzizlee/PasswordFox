using Abp.Domain.Entities;
using NetWorkPassServer.Domain.DeviceInterfaces;

namespace NetWorkPassServer.Domain.InterfaceMetrics;
public sealed class InterfaceMetric
{
    private InterfaceMetric()
    {
    }

    public InterfaceMetric(
        Guid deviceInterfaceId,
        long rxBytes,
        long txBytes)
    {
        DeviceInterfaceId = deviceInterfaceId;

        RxBytes = rxBytes;

        TxBytes = txBytes;

        CollectedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid DeviceInterfaceId { get; private set; }

    public long RxBytes { get; private set; }

    public long TxBytes { get; private set; }

    public long? RxPackets { get; private set; }

    public long? TxPackets { get; private set; }

    public long? RxErrors { get; private set; }

    public long? TxErrors { get; private set; }

    public double? UtilizationPercent { get; private set; }

    public DateTime CollectedAt { get; private set; }

    public DeviceInterface DeviceInterface { get; private set; } = default!;
}