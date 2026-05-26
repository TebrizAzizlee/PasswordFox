using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Dtos.DeviesDtos;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace NetWorkPassServer.Application.Devices;
public sealed record DeviceMonitoringQuery(Guid Id)
    : IRequest<ServiceResult<DeviceMonitoringDto>>;



internal sealed class DeviceMonitoringQueryHandler(
    IPasswordDbContext context)
    : IRequestHandler<
        DeviceMonitoringQuery,
        ServiceResult<DeviceMonitoringDto>>
{
    public async Task<ServiceResult<DeviceMonitoringDto>> Handle(
        DeviceMonitoringQuery request,
        CancellationToken cancellationToken)
    {
        var device = await context.Devices
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new DeviceMonitoringDto(
                x.Id,

                x.BranchId,

                x.Name.Value,

                x.IpAddress.Value,

                x.Type,

                x.Vendor,

                x.Status,

                x.IsCritical,

                x.CpuUsage,

                x.MemoryUsage,

                x.Temperature,

                x.PingLatency,

                x.UptimeSeconds,

                x.LastSeenAt,

                x.IsMonitoringEnabled
            ))
            .SingleOrDefaultAsync(cancellationToken);

        if (device is null)
        {
            return ServiceResult<DeviceMonitoringDto>
                .Failure(
                    "Tapılmadı",
                    "Cihaz tapılmadı",
                    HttpStatusCode.NotFound);
        }

        return ServiceResult<DeviceMonitoringDto>
            .Success(device);
    }
}