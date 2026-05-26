

using GenericRepository;
using NetWorkPassServer.Domain.Alerts;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.Alerts;
public sealed record ResolveAlertCommand(
    Guid AlertId,
    Guid? ResolvedBy,
    string? ResolutionNote)
    : IRequest<ServiceResult>;
internal sealed class ResolveAlertCommandHandler(
    IAlertRepository alertRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<
        ResolveAlertCommand,
        ServiceResult>
{
    public async Task<ServiceResult> Handle(
        ResolveAlertCommand request,
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
                    "Alert artıq bağlanıb",
                    "Bu alert artıq resolved vəziyyətindədir",
                    HttpStatusCode.BadRequest);
        }

        // 🔥 resolve

        alert.Resolve(
            request.ResolvedBy,
            request.ResolutionNote);

        // 🔥 save

        await unitOfWork
            .SaveChangesAsync(
                cancellationToken);

        return ServiceResult
            .Success();
    }
}