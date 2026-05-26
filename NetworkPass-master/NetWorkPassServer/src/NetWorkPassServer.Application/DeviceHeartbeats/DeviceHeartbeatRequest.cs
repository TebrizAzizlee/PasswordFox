

namespace NetWorkPassServer.Application.DeviceHeartbeats;
public sealed record DeviceHeartbeatRequest(

    Guid DeviceId,

    bool IsReachable,

    string? ErrorMessage,

    double? CpuUsage,

    double? DiskUsage,

    double? MemoryUsage,

    double? Temperature,

    long? UptimeSeconds,

    long? ResponseTimeMs
);
