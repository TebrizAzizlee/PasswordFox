using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Alerts;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Dtos;
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
    public sealed record GetCriticalAlertsQuery
     : IRequest<ServiceResult<List<AlertListDto>>>;
}
internal sealed class GetCriticalAlertsQueryHandler(
    IPasswordDbContext context)
    : IRequestHandler<
        GetCriticalAlertsQuery,
        ServiceResult<List<AlertListDto>>>
{
    public async Task<ServiceResult<List<AlertListDto>>> Handle(
        GetCriticalAlertsQuery request,
        CancellationToken cancellationToken)
    {
        var alerts = await context.Alerts
            .AsNoTracking()
            .Where(x => x.Status!=AlertStatus.Resolved &&
                x.Severity == AlertSeverity.Critical)
            .OrderByDescending(x => x.TriggeredAt)
            .Select(x => new AlertListDto(
                x.Id,
                x.DeviceId,
                x.BranchId,
                x.Device.Name.Value,
                x.Branch.Name.Value,
                x.Type,
                x.Severity,
                x.Status,
                x.Message,
                x.TriggeredAt,
                x.ResolvedAt
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<AlertListDto>>
            .Success(alerts);
    }
}