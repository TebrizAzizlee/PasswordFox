using NetWorkPassServer.Domain.Branches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Dtos.BranchDtos;
public sealed record BranchRuntimeSnapshotDto
(Guid BranchId,

    string BranchName,

    BranchStatus Status,

    int TotalDeviceCount,

    int OnlineDeviceCount,

    int OfflineDeviceCount,

    int DegradedDeviceCount,

    int AlertCount,

    int HealthScore,

    bool IsInMaintenanceMode,

    DateTime? LastSeenAt);