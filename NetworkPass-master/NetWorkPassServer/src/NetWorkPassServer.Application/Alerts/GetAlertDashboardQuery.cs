using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Alerts;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Dtos;
using NetWorkPassServer.Application.Dtos.AlertsDtos;
using NetWorkPassServer.Domain.Alerts;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace NetWorkPassServer.Application.Alerts
{
    public sealed record GetAlertDashboardQuery
      : IRequest<ServiceResult<AlertDashboardDto>>;
}
internal sealed class GetAlertDashboardQueryHandler(
    IPasswordDbContext context)
    : IRequestHandler<
        GetAlertDashboardQuery,
        ServiceResult<AlertDashboardDto>>
{
    public async Task<ServiceResult<AlertDashboardDto>> Handle(
        GetAlertDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var activeAlertsQuery = context.Alerts
            .AsNoTracking()
            .Where(x => !x.IsResolved);

        var totalActiveAlerts =
            await activeAlertsQuery.CountAsync(
                cancellationToken);

        var criticalAlerts =
            await activeAlertsQuery.CountAsync(
                x => x.Severity == AlertSeverity.Critical,
                cancellationToken);

        var warningAlerts =
            await activeAlertsQuery.CountAsync(
                x => x.Severity == AlertSeverity.Warning,
                cancellationToken);

        var infoAlerts =
            await activeAlertsQuery.CountAsync(
                x => x.Severity == AlertSeverity.Info,
                cancellationToken);

        var latestAlerts = await activeAlertsQuery
            .OrderByDescending(x => x.TriggeredAt)
            .Take(10)
            .Select(x => new AlertListDto(
                x.Id,
                x.DeviceId,
                x.BranchId,
                x.Device.Name.Value,
                x.Branch.Name,
                x.Type,
                x.Severity,
                x.Message,
                x.IsResolved,
                x.TriggeredAt,
                x.ResolvedAt
            ))
            .ToListAsync(cancellationToken);

        var dto = new AlertDashboardDto(
            totalActiveAlerts,
            criticalAlerts,
            warningAlerts,
            infoAlerts,
            latestAlerts
        );

        return ServiceResult<AlertDashboardDto>
            .Success(dto);
    }
}