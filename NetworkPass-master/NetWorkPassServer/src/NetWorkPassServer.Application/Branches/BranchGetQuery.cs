using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Dtos.BranchDtos;
using NetWorkPassServer.Domain.Branches;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record BranchGetQuery(Guid Id) : IRequest<ServiceResult<BranchDto>>;

internal sealed class BranchGetQueryHandler(IBranchRepository branchRepository) : IRequestHandler<BranchGetQuery, ServiceResult<BranchDto>>
{
    public async Task<ServiceResult<BranchDto>> Handle(BranchGetQuery request, CancellationToken cancellationToken)
    {

        var branch = await branchRepository
         .Where(x => x.Id == request.Id)
         .Select(x => new BranchDto(
    x.Id,
    x.Name.Value,
    x.Address.City,
    x.Address.District,
    x.Address.FullAddress,
    x.ContactInfo.PhoneNumber1,
    x.ContactInfo.PhoneNumber2,
    x.ContactInfo.Email,
    x.NetworkInfo.WanIp,
    x.NetworkInfo.Subnet,
    x.NetworkInfo.Gateway,
    x.NetworkInfo.DnsServer,
    x.Type,
    x.Status,
    x.TotalDeviceCount,
    x.OnlineDeviceCount,
    x.OfflineDeviceCount,
    x.WarningDeviceCount,
    x.AlertCount,
    x.LastSeenAt,
    x.IsMonitoringEnabled,
    x.IsActive,
    x.CreationTime,
    x.LastModificationTime
))
         .SingleOrDefaultAsync(cancellationToken);

        if (branch is null)
        {
            return ServiceResult<BranchDto>.Failure(
                "Tapılmadı",
                "Şöbə mövcud deyil",
                HttpStatusCode.NotFound);
        }

        return ServiceResult<BranchDto>.Success(branch);
    }
}

