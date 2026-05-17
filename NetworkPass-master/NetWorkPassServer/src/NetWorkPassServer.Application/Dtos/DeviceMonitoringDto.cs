using NetWorkPassServer.Domain.Devices;

namespace NetWorkPassServer.Application.Dtos;

public sealed record DeviceMonitoringDto(
    Guid Id,

    Guid BranchId,

    string Name,

    string IpAddress,

    DeviceType Type,

    DeviceVendor Vendor,

    DeviceStatus Status,

    bool IsCritical,

    double? CpuUsage,

    double? MemoryUsage,

    double? Temperature,

    double? PingLatency,

    long UptimeSeconds,

    DateTime? LastSeenAt,

    bool IsMonitoringEnabled
);