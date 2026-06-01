
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Dtos.BranchDtos;
using NetWorkPassServer.Application.Services;
using NetWorkPassServer.Domain.Alerts;
using NetWorkPassServer.Domain.Devices;

namespace NetWorkPassServer.Infrastructure.Services;

internal sealed class BranchStatsService(
    IPasswordDbContext context,
    IUnitOfWork unitOfWork,
    IBranchRealtimeNotifier realtimeNotifier)
    : IBranchStatsService
{
    public async Task RecalculateAsync(
        Guid branchId,
        CancellationToken cancellationToken)
    {
        var branch = await context.Branches
            .FirstOrDefaultAsync(
                x =>
                    x.Id == branchId &&
                    !x.IsDeleted,
                cancellationToken);

        if (branch is null)
        {
            return;
        }
        var deviceStatuses = await context.Devices
    .Where(x =>
        !x.IsDeleted &&
        x.BranchId == branchId)
    .Select(x => new
    {
        x.Id,
        x.Status
    })
    .ToListAsync(cancellationToken);

        foreach (var d in deviceStatuses)
        {
            Console.WriteLine(
                $"Device: {d.Id} Status: {d.Status}");
        }

        if (!branch.IsActive)
        {
            return;
        }

        var deviceStats = await context.Devices
            .AsNoTracking()
            .Where(x =>!x.IsDeleted &&
                x.BranchId == branchId &&
                x.IsActive)
            .GroupBy(x => 1)
            .Select(g => new
            {
                Total =
                    g.Count(),

                Online =
                    g.Count(x =>
                        x.Status ==
                        DeviceStatus.Online),

                Offline =
                    g.Count(x =>
                        x.Status ==
                        DeviceStatus.Offline),

                Degraded =
                    g.Count(x =>
                        x.Status ==
                        DeviceStatus.Degraded),

                LastSeenAt =
                    g.Max(x =>
                        x.LastSeenAt)
            })
            .FirstOrDefaultAsync(cancellationToken);

        var alertStats = await context.Alerts
            .AsNoTracking()
            .Where(x =>
                x.BranchId == branchId &&
                x.Status!=AlertStatus.Resolved)
            .GroupBy(x => 1)
            .Select(g => new
            {
                AlertCount =
                    g.Count(),

                CriticalAlerts =
                    g.Count(x =>
                        x.Severity ==
                        AlertSeverity.Critical)
            })
            .FirstOrDefaultAsync(cancellationToken);

        var total =
            deviceStats?.Total ?? 0;

        var online =
            deviceStats?.Online ?? 0;

        var offline =
            deviceStats?.Offline ?? 0;

        var degraded =
            deviceStats?.Degraded ?? 0;

        var alertCount =
            alertStats?.AlertCount ?? 0;

        var criticalAlerts =
            alertStats?.CriticalAlerts ?? 0;

        var lastSeenAt =
            deviceStats?.LastSeenAt;

        // HEALTH SCORE
        Console.WriteLine($"BranchId={branchId}");
        Console.WriteLine($"Total={total}");
        Console.WriteLine($"Online={online}");
        Console.WriteLine($"Offline={offline}");
        Console.WriteLine($"Degraded={degraded}");
        var healthScore = 100;

        healthScore -= offline * 30;

        healthScore -= degraded * 10;

        healthScore -= criticalAlerts * 20;

        if (healthScore < 0)
        {
            healthScore = 0;
        }

        branch.UpdateStats(
            total,
            online,
            offline,
            degraded,
            alertCount,
            healthScore);
        Console.WriteLine($"Branch Status={branch.Status}");
        branch.UpdateLastSeenAt(
            lastSeenAt);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        var snapshot =
              new BranchRuntimeSnapshotDto(
                  branch.Id,

                  branch.Name.Value,

                  branch.Status,

                  total,

                  online,

                  offline,

                  degraded,

                  alertCount,

                  healthScore,

                  branch.IsInMaintenanceMode,

                  lastSeenAt);
        await realtimeNotifier
           .BranchStatsChangedAsync(
               snapshot,
               cancellationToken);
    }
}

