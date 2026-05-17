using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Dtos.DashboardDtos;
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
        var totalBranches =
            await context.Branches.AsNoTracking()
                .CountAsync(cancellationToken);

        var onlineBranches =
            await context.Branches.AsNoTracking()
                .CountAsync(
                    x => x.Status == BranchStatus.Online,
                    cancellationToken);

        var offlineBranches =
            await context.Branches.AsNoTracking()
                .CountAsync(
                    x => x.Status == BranchStatus.Offline,
                    cancellationToken);

        var warningBranches =
            await context.Branches.AsNoTracking()
                .CountAsync(
                    x => x.Status == BranchStatus.Warning,
                    cancellationToken);

        var totalDevices =
            await context.Devices.AsNoTracking()
                .CountAsync(cancellationToken);

        var onlineDevices =
            await context.Devices.AsNoTracking()
                .CountAsync(
                    x => x.Status == DeviceStatus.Online,
                    cancellationToken);

        var offlineDevices =
            await context.Devices.AsNoTracking()
                .CountAsync(
                    x => x.Status == DeviceStatus.Offline,
                    cancellationToken);

        var warningDevices =
            await context.Devices.AsNoTracking()
                .CountAsync(
                    x => x.Status == DeviceStatus.Warning,
                    cancellationToken);

        // TEMP
        // Alert entity hazır olmayıbsa 0 saxla

        int activeAlerts = 0;

        int criticalAlerts = 0;

        int warningAlerts = 0;

        var dto = new DashboardSummaryDto(
            totalBranches,
            onlineBranches,
            offlineBranches,
            warningBranches,
            totalDevices,
            onlineDevices,
            offlineDevices,
            warningDevices,
            activeAlerts,
            criticalAlerts,
            warningAlerts
        );

        return ServiceResult<DashboardSummaryDto>
            .Success(dto);
    }
}