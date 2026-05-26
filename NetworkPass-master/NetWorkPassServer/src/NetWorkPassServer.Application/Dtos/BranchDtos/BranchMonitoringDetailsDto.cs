using NetWorkPassServer.Application.Dtos.DeviesDtos;
using NetWorkPassServer.Domain.Branches;

namespace NetWorkPassServer.Application.Dtos.BranchDtos;

public sealed record BranchMonitoringDetailsDto(
    Guid Id,

    string Name,

    BranchStatus Status,

    int TotalDeviceCount,

    int OnlineDeviceCount,

    int OfflineDeviceCount,

    int DegradedDeviceCount,

    int AlertCount,

    bool IsMonitoringEnabled,

    bool IsInMaintenanceMode,

    DateTime? LastSeenAt,

    List<DeviceMonitoringItemDto> Devices
);