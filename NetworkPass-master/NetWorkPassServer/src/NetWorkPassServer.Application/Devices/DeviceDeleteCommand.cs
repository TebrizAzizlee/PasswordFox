using GenericRepository;
using NetWorkPassServer.Domain.Devices;
using SharedLibrary;
using SharedLibrary.Abstractions.Entity;
using SharedLibrary.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace NetWorkPassServer.Application.Devices;
public sealed record DeviceDeleteCommand(Guid Id)
    : IRequest<ServiceResult>;
internal sealed class DeviceDeleteCommandHandler(
    IDeviceRepository deviceRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeviceDeleteCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(
        DeviceDeleteCommand request,
        CancellationToken cancellationToken)
    {
        // 🔥 1. Device var?
        var device = await deviceRepository.FirstOrDefaultAsync(
            x => x.Id == request.Id,
            cancellationToken);

        if (device is null)
        {
            return ServiceResult.Failure(
                "Tapılmadı",
                "Device mövcud deyil",
                HttpStatusCode.NotFound);
        }

        // 🔥 2. artıq silinib?
        if (device.IsDeleted)
        {
            return ServiceResult.Failure(
                "Artıq silinib",
                "Device artıq silinmişdir",
                HttpStatusCode.BadRequest);
        }
        var userId = SystemUser.Id; // necə inject edirsənsə
        var now = DateTimeOffset.UtcNow;
        // 🔥 3. soft delete
        device.Delete(new IdentityId(userId),now); // Entity method

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult.Success();
    }
}