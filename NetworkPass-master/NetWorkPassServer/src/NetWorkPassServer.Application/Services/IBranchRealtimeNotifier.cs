using NetWorkPassServer.Application.Dtos.BranchDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Services;
public interface IBranchRealtimeNotifier
{
    Task BranchStatsChangedAsync(
        BranchRuntimeSnapshotDto snapshot,
        CancellationToken cancellationToken);

    Task BranchMaintenanceEnabledAsync(
        Guid branchId,
        CancellationToken cancellationToken);

    Task BranchMaintenanceDisabledAsync(
        Guid branchId,
        CancellationToken cancellationToken);

    Task BranchMonitoringEnabledAsync(
        Guid branchId,
        CancellationToken cancellationToken);

    Task BranchMonitoringDisabledAsync(
        Guid branchId,
        CancellationToken cancellationToken);
}
