using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Branches.ValueObjects;
using NetWorkPassServer.Domain.Devices;

namespace NetWorkPassServer.Application.Dtos.BranchDtos;
public sealed record BranchListDto
(
     Guid Id ,
     string Code,
     string Name ,

     string City,
     BranchType Type ,

     BranchStatus Status ,

     int TotalDeviceCount ,
     int OnlineDeviceCount ,
     int DegradedDeviceCount,
     int OfflineDeviceCount ,

     int AlertCount ,
     int HealthScore,
     bool IsActive,
     DateTime? LastSeenAt 
);
