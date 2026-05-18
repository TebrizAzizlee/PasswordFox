using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Dtos.DeviesDtos;
using NetWorkPassServer.Domain.Devices;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace NetWorkPassServer.Application.Devices;
public sealed record DeviceGetByIdQuery(Guid Id)
    : IRequest<ServiceResult<DeviceDetailDto>>;

internal sealed class DeviceGetByIdQueryHandler(
    IDeviceRepository deviceRepository
) : IRequestHandler<DeviceGetByIdQuery, ServiceResult<DeviceDetailDto>>
{
    public async Task<ServiceResult<DeviceDetailDto>> Handle(
        DeviceGetByIdQuery request,
        CancellationToken cancellationToken)
    {
        var device = await deviceRepository
            .Where(x => x.Id == request.Id)
            .AsNoTracking()
            .Select(x => new DeviceDetailDto(
                x.Id,
                x.BranchId,
                x.Branch.Name.Value, // 🔥 join burda olur
                x.Name.Value,
                x.IpAddress.Value,
                x.Type.ToString(),
                x.Description,
                x.IsActive,
                x.CreationTime
            ))
            .SingleOrDefaultAsync(cancellationToken);

        if (device is null)
        {
            return ServiceResult<DeviceDetailDto>.Failure(
                "Tapılmadı",
                "Device mövcud deyil",
                HttpStatusCode.NotFound);
        }

        return ServiceResult<DeviceDetailDto>.Success(device);
    }
}