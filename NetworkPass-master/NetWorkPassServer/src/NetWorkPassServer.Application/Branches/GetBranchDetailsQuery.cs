using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Dtos.BranchDtos;
using NetWorkPassServer.Domain.Branches;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record GetBranchDetailsQuery(Guid Id) : IRequest<ServiceResult<BranchDetailsDto>>;

internal sealed class BranchGetQueryHandler(IBranchRepository branchRepository) : IRequestHandler<GetBranchDetailsQuery, ServiceResult<BranchDetailsDto>>
{
    public async Task<ServiceResult<BranchDetailsDto>> Handle(GetBranchDetailsQuery request, CancellationToken cancellationToken)
    {

        var branch = await branchRepository
         .Where(x => x.Id == request.Id && !x.IsDeleted).AsNoTracking()
         .Select(x => new BranchDetailsDto(
    x.Id,
    x.Code,
     x.Name.Value,
    x.Description,
    x.Type,
    x.Status,

    // ADDRESS
    x.Address.City,
    x.Address.District,
    x.Address.FullAddress,

    // CONTACT
    x.ContactInfo.PhoneNumber1,
    x.ContactInfo.PhoneNumber2,
    x.ContactInfo.Email,

    // NETWORK
    x.NetworkInfo.WanIp,
    x.NetworkInfo.Subnet,
    x.NetworkInfo.Gateway,
    x.NetworkInfo.DnsServer,

    // STATS
    x.TotalDeviceCount,
    x.OnlineDeviceCount,
    x.OfflineDeviceCount,
    x.DegradedDeviceCount,
    x.AlertCount,

    x.HealthScore,

    // STATE
    x.LastSeenAt,
    x.IsMonitoringEnabled,
    x.IsInMaintenanceMode,
    x.IsActive,


    // AUDIT
    
    x.CreationTime,
    x.LastModificationTime

))
         .SingleOrDefaultAsync(cancellationToken);

        if (branch is null)
        {
            return ServiceResult<BranchDetailsDto>.Failure(
                "Tapılmadı",
                "Şöbə mövcud deyil",
                HttpStatusCode.NotFound);
        }

        return ServiceResult<BranchDetailsDto>.Success(branch);
    }
}

