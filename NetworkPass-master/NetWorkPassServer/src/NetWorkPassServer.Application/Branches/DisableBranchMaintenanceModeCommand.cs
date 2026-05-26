using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record DisableBranchMaintenanceModeCommand(Guid Id)
    : IRequest<ServiceResult>;

internal sealed class DisableBranchMaintenanceModeCommandHandler(
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<
        DisableBranchMaintenanceModeCommand,
        ServiceResult>
{
    public async Task<ServiceResult> Handle(
        DisableBranchMaintenanceModeCommand request,
        CancellationToken cancellationToken)
    {
        var branch = await branchRepository
            .FirstOrDefaultAsync(
                x =>
                    x.Id == request.Id &&
                    !x.IsDeleted,
                cancellationToken);

        if (branch is null)
        {
            return ServiceResult.Failure(
                "Tapılmadı",
                "Şöbə tapılmadı",
                HttpStatusCode.NotFound);
        }

        branch.DisableMaintenanceMode();

        await unitOfWork
            .SaveChangesAsync(
                cancellationToken);

        return ServiceResult.Success();
    }
}
