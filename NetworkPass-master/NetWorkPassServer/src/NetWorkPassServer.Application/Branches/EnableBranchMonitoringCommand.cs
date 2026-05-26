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
public sealed record EnableBranchMonitoringCommand(Guid Id)
    : IRequest<ServiceResult>;

internal sealed class EnableBranchMonitoringCommandHandler(
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<
        EnableBranchMonitoringCommand,
        ServiceResult>
{
    public async Task<ServiceResult> Handle(
        EnableBranchMonitoringCommand request,
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

        if (!branch.IsActive)
        {
            return ServiceResult.Failure(
                "Aktiv deyil",
                "Deaktiv şöbə üçün monitoring aktiv edilə bilməz",
                HttpStatusCode.BadRequest);
        }

        branch.EnableMonitoring();

        await unitOfWork
            .SaveChangesAsync(
                cancellationToken);

        return ServiceResult.Success();
    }
}