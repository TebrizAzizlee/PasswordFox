using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Dtos.BranchDtos;
using NetWorkPassServer.Application.Dtos.DeviesDtos;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record BranchMonitoringListQuery
    : IRequest<ServiceResult<List<BranchMonitoringListDto>>>;

internal sealed class BranchMonitoringListQueryHandler(
    IPasswordDbContext context)
    : IRequestHandler<
        BranchMonitoringListQuery,
        ServiceResult<List<BranchMonitoringListDto>>>
{
    public async Task<ServiceResult<List<BranchMonitoringListDto>>> Handle(
        BranchMonitoringListQuery request,
        CancellationToken cancellationToken)
    {
        var branches = await context.Branches
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x=>x.Name.Value)
            .Select(x => new BranchMonitoringListDto(
                x.Id,
                x.Name.Value,
                x.Status,
                x.TotalDeviceCount,
                x.OnlineDeviceCount,
                x.OfflineDeviceCount,
                x.DegradedDeviceCount,
                x.AlertCount,
                x.LastSeenAt,
                x.IsMonitoringEnabled,
                x.IsInMaintenanceMode
                )).ToListAsync(cancellationToken);


        return ServiceResult<
           List<BranchMonitoringListDto>>
           .Success(branches);
    }
}