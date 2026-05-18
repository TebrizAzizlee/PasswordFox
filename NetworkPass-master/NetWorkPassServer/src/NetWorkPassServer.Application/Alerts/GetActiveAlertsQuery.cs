using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Dtos;
using SharedLibrary;
using TS.MediatR;

namespace NetWorkPassServer.Application.Alerts;

public sealed record GetActiveAlertsQuery
    : IRequest<ServiceResult<List<AlertListDto>>>;
internal sealed class GetActiveAlertsQueryHandler(
    IPasswordDbContext context)
    : IRequestHandler<
        GetActiveAlertsQuery,
        ServiceResult<List<AlertListDto>>>
{
    public async Task<ServiceResult<List<AlertListDto>>> Handle(
        GetActiveAlertsQuery request,
        CancellationToken cancellationToken)
    {
        var alerts = await context.Alerts
            .AsNoTracking()
            .Where(x => !x.IsResolved)
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