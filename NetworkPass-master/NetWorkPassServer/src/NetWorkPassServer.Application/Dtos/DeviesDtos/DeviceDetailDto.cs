using NetWorkPassServer.Domain.Devices;


namespace NetWorkPassServer.Application.Dtos.DeviesDtos;
public sealed record DeviceDetailDto(
    Guid Id,
    Guid BranchId,
    string BranchName,
    string Name,
    string IpAddress,
    DeviceType Type,
    string Model,
    DeviceStatus Status,
    string? Description,
    DateTime? LastSeenAt,
    bool IsMonitoringEnabled,
    bool IsActive,
   DateTime CreationTime,
    DateTime? LastModificationTime
);