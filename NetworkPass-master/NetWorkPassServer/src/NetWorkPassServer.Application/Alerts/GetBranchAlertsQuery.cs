using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Alerts;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Dtos;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace NetWorkPassServer.Application.Alerts
{
    public sealed record GetBranchAlertsQuery(
      Guid BranchId)
      : IRequest<ServiceResult<List<AlertListDto>>>;
}
internal sealed class GetBranchAlertsQueryHandler(
    IPasswordDbContext context)
    : IRequestHandler<
        GetBranchAlertsQuery,
        ServiceResult<List<AlertListDto>>>
{
    public async Task<ServiceResult<List<AlertListDto>>> Handle(
        GetBranchAlertsQuery request,
        CancellationToken cancellationToken)
    {
        var alerts = await context.Alerts
            .AsNoTracking()
            .Where(x => x.BranchId == request.BranchId)
            .OrderByDescending(x => x.TriggeredAt)
            .Select(x => new AlertListDto(
                x.Id,
                x.DeviceId,
                x.BranchId,
                x.Device.Name.Value,
                x.Branch.Name.Value,
                x.Type,
                x.Severity,
                x.Message,
                x.IsResolved,
                x.TriggeredAt,
                x.ResolvedAt
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<AlertListDto>>
            .Success(alerts);
    }
}