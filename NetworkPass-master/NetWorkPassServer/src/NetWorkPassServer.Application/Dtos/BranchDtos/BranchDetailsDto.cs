using NetWorkPassServer.Domain.Branches;

namespace NetWorkPassServer.Application.Dtos.BranchDtos;
public sealed record BranchDetailsDto(
    Guid Id,

    string Code,

    string Name,

    string? Description,

    BranchType Type,

    BranchStatus Status,

    // ADDRESS

    string City,

    string District,

    string FullAddress,

    // CONTACT

    string PhoneNumber1,

    string? PhoneNumber2,

    string Email,

    // NETWORK

    string? WanIp,

    string? Subnet,

    string? Gateway,

    string? DnsServer,

    // STATS

    int TotalDeviceCount,

    int OnlineDeviceCount,

    int OfflineDeviceCount,

    int DegradedDeviceCount,

    int AlertCount,

    int HealthScore,

    // STATE

    DateTime? LastSeenAt,

    bool IsMonitoringEnabled,

    bool IsInMaintenanceMode,

    bool IsActive,

    // AUDIT

    DateTime CreationTime,

    DateTime? LastModificationTime
);
