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
public sealed record EnableBranchMaintenanceModeCommand(Guid Id)
    : IRequest<ServiceResult>;

internal sealed class EnableBranchMaintenanceModeCommandHandler(
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<
        EnableBranchMaintenanceModeCommand,
        ServiceResult>
{
    public async Task<ServiceResult> Handle(
        EnableBranchMaintenanceModeCommand request,
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

        if (!branch.IsMonitoringEnabled)
        {
            return ServiceResult.Failure(
                "Monitoring deaktivdir",
                "Monitoring deaktiv olduqda maintenance aktiv edilə bilməz",
                HttpStatusCode.BadRequest);
        }

        branch.EnableMaintenanceMode();

        await unitOfWork
            .SaveChangesAsync(
                cancellationToken);

        return ServiceResult.Success();
    }
}
