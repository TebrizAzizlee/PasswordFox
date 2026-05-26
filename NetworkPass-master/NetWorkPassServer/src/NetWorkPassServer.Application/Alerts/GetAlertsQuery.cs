using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Dtos;
using SharedLibrary;
using TS.MediatR;

namespace NetWorkPassServer.Application.Alerts;
public sealed record GetAlertsQuery(
    int Page = 1,
    int PageSize = 20)
    : IRequest<ServiceResult<List<AlertListDto>>>;

internal sealed class GetAlertsQueryHandler(
    IPasswordDbContext context)
    : IRequestHandler<
        GetAlertsQuery,
        ServiceResult<List<AlertListDto>>>
{
    public async Task<
        ServiceResult<List<AlertListDto>>>
        Handle(
            GetAlertsQuery request,
            CancellationToken cancellationToken)
    {
        var page =
            request.Page < 1
                ? 1
                : request.Page;

        var pageSize =
            request.PageSize switch
            {
                < 1 => 20,
                > 100 => 100,
                _ => request.PageSize
            };

        var alerts = await context.Alerts
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.TriggeredAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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

        return ServiceResult<
            List<AlertListDto>>
            .Success(alerts);
    }
}
