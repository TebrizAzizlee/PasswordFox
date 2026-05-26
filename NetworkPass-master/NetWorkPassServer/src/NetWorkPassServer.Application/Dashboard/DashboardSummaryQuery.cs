using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Dtos.DashboardDtos;
using NetWorkPassServer.Domain.Alerts;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Devices;
using SharedLibrary;

using TS.MediatR;

namespace NetWorkPassServer.Application.Dashboard;
public sealed record DashboardSummaryQuery
    : IRequest<ServiceResult<DashboardSummaryDto>>;

internal sealed class DashboardSummaryQueryHandler(
    IPasswordDbContext context)
    : IRequestHandler<
        DashboardSummaryQuery,
        ServiceResult<DashboardSummaryDto>>
{
    public async Task<ServiceResult<DashboardSummaryDto>> Handle(
        DashboardSummaryQuery request,
        CancellationToken cancellationToken)
    {
        // 🔥 BRANCH STATS
       var branchStats=
            await context.Branches.AsNoTracking().Where(x=>!x.IsDeleted &&
            x.IsActive).GroupBy(x => 1).Select(g => new
            {
                Total=g.Count(),
                Online=g.Count(x=>x.Status==BranchStatus.Online),
                Offline=g.Count(x=>x.Status==BranchStatus.Offline),
                Degraded=g.Count(x=>x.Status!=BranchStatus.Degraded)

            }).FirstOrDefaultAsync(cancellationToken);

        // 🔥 DEVICE STATS

        var deviceStats =
            await context.Devices
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.IsActive)
                .GroupBy(x => 1)
                .Select(g => new
                {
                    Total = g.Count(),


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
                                DeviceStatus.Degraded)
                })
                .FirstOrDefaultAsync(
                    cancellationToken);

        // TEMP
        // Alert entity hazır olmayıbsa 0 saxla
        // 🔥 ALERT STATS

        var alertStats =
            await context.Alerts
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.Status !=
                        AlertStatus.Resolved)
                .GroupBy(x => 1)
                .Select(g => new
                {
                    Active =
                        g.Count(),

                    Critical =
                        g.Count(x =>
                            x.Severity ==
                                AlertSeverity.Critical),

                    Warning =
                        g.Count(x =>
                            x.Severity ==
                                AlertSeverity.Warning)
                })
                .FirstOrDefaultAsync(
                    cancellationToken);

        var dto = new DashboardSummaryDto(
            //Branches
                 branchStats?.Total ?? 0,
                 branchStats?.Online ?? 0,
                branchStats?.Offline ?? 0,
                branchStats?.Degraded ?? 0,
                // DEVICES

                deviceStats?.Total ?? 0,

                deviceStats?.Online ?? 0,

                deviceStats?.Offline ?? 0,

                deviceStats?.Degraded ?? 0,

                // ALERTS

                alertStats?.Active ?? 0,

                alertStats?.Critical ?? 0,

                alertStats?.Warning ?? 0

        );

        return ServiceResult<DashboardSummaryDto>
            .Success(dto);
    }
}