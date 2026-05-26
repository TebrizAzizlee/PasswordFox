using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Dtos.AlertsDtos;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.Alerts;
public sealed record GetAlertByIdQuery(
    Guid Id)
    : IRequest<ServiceResult<AlertDetailDto>>;

internal sealed class GetAlertByIdQueryHandler(
    IPasswordDbContext context)
    : IRequestHandler<
        GetAlertByIdQuery,
        ServiceResult<AlertDetailDto>>
{
    public async Task<
        ServiceResult<AlertDetailDto>>
        Handle(
            GetAlertByIdQuery request,
            CancellationToken cancellationToken)
    {
        var alert = await context.Alerts
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.Id == request.Id)
            .Select(x =>
                new AlertDetailDto(

                    x.Id,

                    x.DeviceId,

                    x.BranchId,

                    x.Device.Name.Value,

                    x.Branch.Name.Value,

                    x.Type,

                    x.Severity,

                    x.Status,

                    x.Source,

                    x.Title,

                    x.Message,

                    x.Fingerprint,

                    x.OccurrenceCount,

                    x.TriggeredAt,

                    x.AcknowledgedAt,

                    x.AcknowledgedBy,

                    x.ResolvedAt,

                    x.ResolvedBy,

                    x.ResolutionNote
                ))
            .FirstOrDefaultAsync(
                cancellationToken);

        if (alert is null)
        {
            return ServiceResult<
                AlertDetailDto>
                .Failure(
                    "Tapılmadı",
                    "Alert tapılmadı",
                    HttpStatusCode.NotFound);
        }

        return ServiceResult<
            AlertDetailDto>
            .Success(alert);
    }
}