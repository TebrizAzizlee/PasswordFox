using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Devices;
using SharedLibrary;
using SharedLibrary.Abstractions.Entity;
using SharedLibrary.Constants;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record BranchDeleteCommand(Guid Id) : IRequest<ServiceResult>;

internal sealed class BranchDeleteCommandHnadler(IBranchRepository branchRepository, IDeviceRepository deviceRepository, IUnitOfWork unitOfWork) : IRequestHandler<BranchDeleteCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(BranchDeleteCommand request, CancellationToken cancellationToken)
    {
        var branch = await branchRepository.FirstOrDefaultAsync(i => i.Id==request.Id, cancellationToken);

        if (branch is null)
        {
            return ServiceResult.Failure("Tapılmadı", "Şöbə tapılmadı", HttpStatusCode.NotFound);
        }
        bool hasDevices =
           await deviceRepository.AnyAsync(
               x => x.BranchId == request.Id &&
                    !x.IsDeleted,
               cancellationToken);

        if (hasDevices)
        {
            return ServiceResult.Failure(
                "Silmək mümkün deyil",
                "Şöbədə aktiv cihazlar mövcuddur",
                HttpStatusCode.BadRequest);
        }
        branch.MarkAsDeleted();
        
       
        await unitOfWork.SaveChangesAsync(
          cancellationToken);


        return ServiceResult.Success();
    }
}

