using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Dtos.BranchDtos;
using NetWorkPassServer.Application.Dtos.DeviesDtos;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record BranchMonitoringDetailsQuery(
    Guid Id)
    : IRequest<ServiceResult<BranchMonitoringDetailsDto>>;


internal sealed class BranchMonitoringDetailsQueryHandler(
    IPasswordDbContext context)
    : IRequestHandler<
        BranchMonitoringDetailsQuery,
        ServiceResult<BranchMonitoringDetailsDto>>
{
    public async Task<
        ServiceResult<BranchMonitoringDetailsDto>>
        Handle(
            BranchMonitoringDetailsQuery request,
            CancellationToken cancellationToken)
    {
        var branch = await context.Branches
            .AsNoTracking()
            .Where(x =>
                x.Id == request.Id &&
                x.IsActive)
            .Select(x =>
                new BranchMonitoringDetailsDto(
                    x.Id,

                    x.Name.Value,

                    x.Status,

                    x.TotalDeviceCount,

                    x.OnlineDeviceCount,

                    x.OfflineDeviceCount,

                    x.DegradedDeviceCount,

                    x.AlertCount,

                    x.IsMonitoringEnabled,

                    x.IsInMaintenanceMode,

                    x.LastSeenAt,

                    x.Devices
                        .Where(d =>
                            d.IsActive &&
                            !d.IsDeleted)
                        .OrderBy(d =>
                            d.Name.Value)
                        .Select(d =>
                            new DeviceMonitoringItemDto(
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
            return ServiceResult<
                BranchMonitoringDetailsDto>
                .Failure(
                    "Tapılmadı",
                    "Şöbə tapılmadı",
                    HttpStatusCode.NotFound);
        }

        return ServiceResult<
            BranchMonitoringDetailsDto>
            .Success(branch);
    }
}