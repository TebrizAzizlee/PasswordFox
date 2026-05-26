using NetWorkPassServer.Application.Dtos.DeviesDtos;
using NetWorkPassServer.Domain.Branches;


namespace NetWorkPassServer.Application.Dtos.BranchDtos;
public sealed record BranchMonitoringListDto(
    Guid Id,

    string Name,

    BranchStatus Status,

    int TotalDeviceCount,

    int OnlineDeviceCount,

    int OfflineDeviceCount,

    int DegradedDeviceCount,

    int AlertCount,

    DateTime? LastSeenAt,

    bool IsMonitoringEnabled,
    bool IsInMaintenanceMode
    
);