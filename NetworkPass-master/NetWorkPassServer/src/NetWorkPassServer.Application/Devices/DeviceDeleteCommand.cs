using GenericRepository;
using NetWorkPassServer.Domain.Devices;
using SharedLibrary;
using SharedLibrary.Constants;

using System.Net;

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
            x => x.Id == request.Id && !x.IsDeleted,
            cancellationToken);

        if (device is null)
        {
            return ServiceResult.Failure(
                "Tapılmadı",
                "Device mövcud deyil",
                HttpStatusCode.NotFound);
        }

        device.MarkAsDeleted();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult.Success();
    }
}