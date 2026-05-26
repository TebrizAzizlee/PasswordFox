using GenericRepository;
using NetWorkPassServer.Domain.Alerts;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.Alerts;
public sealed record AcknowledgeAlertCommand(
    Guid AlertId,
    Guid UserId)
    : IRequest<ServiceResult>;

internal sealed class AcknowledgeAlertCommandHandler(
    IAlertRepository alertRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<
        AcknowledgeAlertCommand,
        ServiceResult>
{
    public async Task<ServiceResult> Handle(
        AcknowledgeAlertCommand request,
        CancellationToken cancellationToken)
    {
        // 🔥 alert exists?

        var alert = await alertRepository
            .FirstOrDefaultAsync(
                x =>
                    x.Id ==
                        request.AlertId &&
                    !x.IsDeleted,
                cancellationToken);

        if (alert is null)
        {
            return ServiceResult
                .Failure(
                    "Tapılmadı",
                    "Alert tapılmadı",
                    HttpStatusCode.NotFound);
        }

        // 🔥 already resolved?

        if (alert.Status ==
            AlertStatus.Resolved)
        {
            return ServiceResult
                .Failure(
                    "Alert bağlanıb",
                    "Resolved alert acknowledge edilə bilməz",
                    HttpStatusCode.BadRequest);
        }

        // 🔥 already acknowledged?

        if (alert.Status ==
            AlertStatus.Acknowledged)
        {
            return ServiceResult
                .Failure(
                    "Alert artıq acknowledge edilib",
                    "Bu alert artıq acknowledge olunub",
                    HttpStatusCode.BadRequest);
        }

        // 🔥 acknowledge

        alert.Acknowledge(
            request.UserId);

        // 🔥 save

        await unitOfWork
            .SaveChangesAsync(
                cancellationToken);

        return ServiceResult
            .Success();
    }
}