using NetWorkPassServer.Domain.Devices;


namespace NetWorkPassServer.Application.Dtos;
public sealed record DeviceMonitoringItemDto(
    Guid Id,

    string Name,

    string IpAddress,

    DeviceType Type,

    DeviceStatus Status,

    double? CpuUsage,

    double? MemoryUsage,

    double? PingLatency,

    DateTime? LastSeenAt
);