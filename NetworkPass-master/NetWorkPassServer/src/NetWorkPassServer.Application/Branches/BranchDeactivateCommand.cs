using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record BranchDeactivateCommand(Guid Id)
    : IRequest<ServiceResult>;

internal sealed class BranchDeactivateCommandHandler(
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<
        BranchDeactivateCommand,
        ServiceResult>
{
    public async Task<ServiceResult> Handle(
        BranchDeactivateCommand request,
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

        branch.Deactivate();

        await unitOfWork
            .SaveChangesAsync(
                cancellationToken);

        return ServiceResult.Success();
    }
}
