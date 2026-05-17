using NetWorkPassServer.Domain.Branches;


namespace NetWorkPassServer.Application.Dtos;
public sealed record BranchDto(
    Guid Id,

    string Name,

    string City,

    string District,

    string FullAddress,

    string PhoneNumber1,

    string? PhoneNumber2,

    string Email,

    string? WanIp,

    string? Subnet,

    string? Gateway,

    string? DnsServer,

    BranchType Type,

    BranchStatus Status,

    int TotalDeviceCount,

    int OnlineDeviceCount,

    int OfflineDeviceCount,

    int WarningDeviceCount,

    int AlertCount,

    DateTime? LastSeenAt,

    bool IsMonitoringEnabled,

    bool IsActive,

    DateTime CreationTime,

    DateTime? LastModificationTime
);