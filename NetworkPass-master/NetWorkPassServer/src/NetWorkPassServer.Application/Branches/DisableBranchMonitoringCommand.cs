using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record DisableBranchMonitoringCommand(Guid Id)
    : IRequest<ServiceResult>;

internal sealed class DisableBranchMonitoringCommandHandler(
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<
        DisableBranchMonitoringCommand,
        ServiceResult>
{
    public async Task<ServiceResult> Handle(
        DisableBranchMonitoringCommand request,
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

        branch.DisableMonitoring();

        await unitOfWork
            .SaveChangesAsync(
                cancellationToken);

        return ServiceResult.Success();
    }
}

