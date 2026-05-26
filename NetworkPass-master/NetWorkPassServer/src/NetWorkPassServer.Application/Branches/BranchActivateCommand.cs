using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record BranchActivateCommand(Guid Id)
    : IRequest<ServiceResult>;

internal sealed class BranchActivateCommandHandler(
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<
        BranchActivateCommand,
        ServiceResult>
{
    public async Task<ServiceResult> Handle(
        BranchActivateCommand request,
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

        branch.Activate();

        await unitOfWork
            .SaveChangesAsync(
                cancellationToken);

        return ServiceResult.Success();
    }
}
