using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Devices;
using NetWorkPassServer.Domain.VpnTunnels;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record BranchDeleteCommand(Guid Id) : IRequest<ServiceResult>;

internal sealed class BranchDeleteCommandHnadler(IBranchRepository branchRepository, IDeviceRepository deviceRepository, IVpnTunnelRepository vpnTunnelRepository, IUnitOfWork unitOfWork) : IRequestHandler<BranchDeleteCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(BranchDeleteCommand request, CancellationToken cancellationToken)
    {
        var branch = await branchRepository.FirstOrDefaultAsync(i => i.Id==request.Id && !i.IsDeleted, cancellationToken);

        if (branch is null)
        {
            return ServiceResult.Failure("Tapılmadı", "Şöbə tapılmadı", HttpStatusCode.NotFound);
        }
        // DEVICE CHECK
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
        // VPN CHECK
        var hasVpnTunnels =
           await vpnTunnelRepository.AnyAsync(
               x =>
                   x.BranchId == request.Id &&
                   !x.IsDeleted,
               cancellationToken);
        if (hasVpnTunnels)
        {
            return ServiceResult.Failure(
                "Silmək mümkün deyil",
                "Şöbədə aktiv VPN tunellər mövcuddur",
                HttpStatusCode.BadRequest);
        }
        branch.MarkAsDeleted();
        
       
        await unitOfWork.SaveChangesAsync(
          cancellationToken);


        return ServiceResult.Success();
    }
}

