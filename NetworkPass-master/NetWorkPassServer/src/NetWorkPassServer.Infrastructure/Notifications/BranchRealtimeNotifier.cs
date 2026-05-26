using Microsoft.AspNetCore.SignalR;
using NetWorkPassServer.Application.Dtos.BranchDtos;
using NetWorkPassServer.Application.Notifications;
using NetWorkPassServer.Application.Services;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Infrastructure.Hubs;

namespace NetWorkPassServer.Infrastructure.Notifications;

internal sealed class BranchRealtimeNotifier(
    IHubContext<MonitoringHub> hubContext)
    : IBranchRealtimeNotifier
{
    public async Task BranchMaintenanceDisabledAsync(Guid branchId, CancellationToken cancellationToken)
    {
        await hubContext.Clients
             .Group($"branch:{branchId}")
             .SendAsync(
                 "branch-maintenance-disabled",
                 new
                 {
                     BranchId = branchId
                 },
                 cancellationToken);
    }

    public async Task BranchMaintenanceEnabledAsync(Guid branchId, CancellationToken cancellationToken)
    {
        await hubContext.Clients
            .Group($"branch:{branchId}")
            .SendAsync(
                "branch-maintenance-enabled",
                new
                {
                    BranchId = branchId
                },
                cancellationToken);
    }

    public async Task BranchMonitoringDisabledAsync(Guid branchId, CancellationToken cancellationToken)
    {
        await hubContext.Clients
             .Group($"branch:{branchId}")
             .SendAsync(
                 "branch-monitoring-disabled",
                 new
                 {
                     BranchId = branchId
                 },
                 cancellationToken);
    }

    public async Task BranchMonitoringEnabledAsync(Guid branchId, CancellationToken cancellationToken)
    {
        await hubContext.Clients
             .Group($"branch:{branchId}")
             .SendAsync(
                 "branch-monitoring-enabled",
                 new
                 {
                     BranchId = branchId
                 },
                 cancellationToken);
    }

    public async Task BranchStatsChangedAsync(BranchRuntimeSnapshotDto snapshot, CancellationToken cancellationToken)
    {
        await hubContext.Clients
           .Group($"branch:{snapshot.BranchId}")
           .SendAsync(
               "branch-stats-updated",
               snapshot,
               cancellationToken);
    }
}