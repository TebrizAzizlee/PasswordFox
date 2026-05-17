using NetWorkPassServer.Domain.Branches;


namespace NetWorkPassServer.Application.Dtos;
public sealed record BranchMonitoringDto(
    Guid Id,

    string Name,

    BranchStatus Status,

    int TotalDeviceCount,

    int OnlineDeviceCount,

    int OfflineDeviceCount,

    int WarningDeviceCount,

    int AlertCount,

    DateTime? LastSeenAt,

    bool IsMonitoringEnabled,

    List<DeviceMonitoringItemDto> Devices
);