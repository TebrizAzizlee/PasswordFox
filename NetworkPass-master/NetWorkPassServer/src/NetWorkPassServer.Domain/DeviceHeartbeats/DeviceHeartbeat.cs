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
        int? responseTimeMs,
        string? errorMessage)
    {
        DeviceId = deviceId;
        Status = status;
        IsReachable = isReachable;
        ResponseTimeMs = responseTimeMs;
        ErrorMessage = errorMessage;

        Timestamp = DateTime.UtcNow;
    }

    public Guid DeviceId { get; private set; }

    public DateTime Timestamp { get; private set; }

    public DeviceStatus Status { get; private set; }

    public bool IsReachable { get; private set; }

    public int? ResponseTimeMs { get; private set; }

    public string? ErrorMessage { get; private set; }

    public Device Device { get; private set; } = default!;
}