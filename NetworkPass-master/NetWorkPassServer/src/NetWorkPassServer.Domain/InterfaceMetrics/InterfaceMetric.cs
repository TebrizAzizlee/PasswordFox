using Abp.Domain.Entities;

namespace NetWorkPassServer.Domain.InterfaceMetrics;
public sealed class InterfaceMetric : Entity<Guid>
{
    public Guid DeviceInterfaceId { get; private set; }

    public DateTime Timestamp { get; private set; }

    public long RxBytes { get; private set; }

    public long TxBytes { get; private set; }

    public long RxPackets { get; private set; }

    public long TxPackets { get; private set; }

    public long RxErrors { get; private set; }

    public long TxErrors { get; private set; }

    public long RxDropped { get; private set; }

    public long TxDropped { get; private set; }
}