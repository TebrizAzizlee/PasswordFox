using Abp.Domain.Entities;
using NetWorkPassServer.Domain.Devices;

namespace NetWorkPassServer.Domain.DeviceHeartbeats;
public sealed class DeviceHeartbeat : Entity<Guid>
{
    private DeviceHeartbeat()
    {
    }

    public DeviceHeartbeat(
        Guid deviceId,
        DeviceStatus status,
        bool isReachable,
        long? responseTimeMs,
        DateTime occurredAtUtc,
        string? errorMessage)
    {
        DeviceId = deviceId;
        Status = status;
        IsReachable = isReachable;
        ResponseTimeMs = responseTimeMs;
        ErrorMessage = errorMessage;
        OccurredAtUtc=occurredAtUtc;
        Timestamp = DateTime.UtcNow;
    }

    public Guid DeviceId { get; private set; }

    public DateTime Timestamp { get; private set; }

    public DeviceStatus Status { get; private set; }

    public bool IsReachable { get; private set; }
    public DateTime OccurredAtUtc {  get; private set; }
    public long? ResponseTimeMs { get; private set; }

    public string? ErrorMessage { get; private set; }

    public Device Device { get; private set; } = default!;
}