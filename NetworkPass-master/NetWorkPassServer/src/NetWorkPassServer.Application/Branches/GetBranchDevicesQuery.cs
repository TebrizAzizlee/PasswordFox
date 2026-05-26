using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Dtos.DeviesDtos;
using NetWorkPassServer.Domain.Devices;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;

public sealed record GetBranchDevicesQuery(
    Guid BranchId,

    DeviceStatus? Status,

    string? Search)
    : IRequest<ServiceResult<List<DeviceMonitoringItemDto>>>;



internal sealed class GetBranchDevicesQueryHandler(
    IPasswordDbContext context)
    : IRequestHandler<
        GetBranchDevicesQuery,
        ServiceResult<List<DeviceMonitoringItemDto>>>
{
    public async Task<
        ServiceResult<List<DeviceMonitoringItemDto>>>
        Handle(
            GetBranchDevicesQuery request,
            CancellationToken cancellationToken)
    {
        var branchExists = await context.Branches
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.Id == request.BranchId &&
                    x.IsActive,
                cancellationToken);

        if (!branchExists)
        {
            return ServiceResult<
                List<DeviceMonitoringItemDto>>
                .Failure(
                    "Tapılmadı",
                    "Şöbə tapılmadı",
                    HttpStatusCode.NotFound);
        }

        var query = context.Devices
            .AsNoTracking()
            .Where(x =>
                x.BranchId == request.BranchId &&
                x.IsActive &&
                !x.IsDeleted);

        if (request.Status.HasValue)
        {
            query = query.Where(x =>
                x.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x =>
                x.Name.Value.Contains(request.Search));
        }

        var devices = await query
            .OrderBy(x => x.Name.Value)
            .Select(x =>
                new DeviceMonitoringItemDto(
                    x.Id,

                    x.Name.Value,

                    x.IpAddress.Value,

                    x.Type,

                    x.Status,

                    x.CpuUsage,

                    x.MemoryUsage,

                    x.PingLatency,

                    x.LastSeenAt
                ))
            .ToListAsync(cancellationToken);

        return ServiceResult<
            List<DeviceMonitoringItemDto>>
            .Success(devices);
    }
}