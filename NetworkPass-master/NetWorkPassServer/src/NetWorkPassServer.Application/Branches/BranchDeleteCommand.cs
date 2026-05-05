using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using SharedLibrary;
using SharedLibrary.Abstractions.Entity;
using SharedLibrary.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record BranchDeleteCommand(Guid Id) : IRequest<ServiceResult>;

internal sealed class BranchDeleteCommandHnadler(IBranchRepository branchRepository, IUnitOfWork unitOfWork) : IRequestHandler<BranchDeleteCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(BranchDeleteCommand request, CancellationToken cancellationToken)
    {
        var branch = await branchRepository.FirstOrDefaultAsync(i => i.Id==request.Id, cancellationToken);
        if (branch is null)
        {
            return ServiceResult.Failure("Tapılmadı", "Şöbə tapılmadı", HttpStatusCode.NotFound);
        }
        var userId = SystemUser.Id; // necə inject edirsənsə
        var now = DateTimeOffset.UtcNow;
        branch.Delete(new IdentityId(userId),now);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult.Success();
    }
}

