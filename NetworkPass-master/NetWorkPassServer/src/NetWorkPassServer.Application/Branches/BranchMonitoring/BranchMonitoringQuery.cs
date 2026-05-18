using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Dtos;
using NetWorkPassServer.Application.Dtos.BranchDtos;
using NetWorkPassServer.Application.Dtos.DeviesDtos;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches.BranchMonitoring;
public sealed record BranchMonitoringQuery(Guid Id)
    : IRequest<ServiceResult<BranchMonitoringDto>>;

internal sealed class BranchMonitoringQueryHandler(
    IPasswordDbContext context)
    : IRequestHandler<
        BranchMonitoringQuery,
        ServiceResult<BranchMonitoringDto>>
{
    public async Task<ServiceResult<BranchMonitoringDto>> Handle(
        BranchMonitoringQuery request,
        CancellationToken cancellationToken)
    {
        var branch = await context.Branches
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new BranchMonitoringDto(
                x.Id,
                x.Name.Value,
                x.Status,
                x.TotalDeviceCount,
                x.OnlineDeviceCount,
                x.OfflineDeviceCount,
                x.WarningDeviceCount,
                x.AlertCount,
                x.LastSeenAt,
                x.IsMonitoringEnabled,

                x.Devices
                    .Where(d => !d.IsDeleted)
                    .OrderBy(d => d.Name.Value)
                    .Select(d => new DeviceMonitoringItemDto(
                        d.Id,
                        d.Name.Value,
                        d.IpAddress.Value,
                        d.Type,
                        d.Status,
                        d.CpuUsage,
                        d.MemoryUsage,
                        d.PingLatency,
                        d.LastSeenAt
                    ))
                    .ToList()
            ))
            .SingleOrDefaultAsync(cancellationToken);

        if (branch is null)
        {
            return ServiceResult<BranchMonitoringDto>.Failure(
                "Tapılmadı",
                "Şöbə tapılmadı",
                HttpStatusCode.NotFound);
        }

        return ServiceResult<BranchMonitoringDto>
            .Success(branch);
    }
}