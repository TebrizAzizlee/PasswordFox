using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.DeviceHeartbeats;
using NetWorkPassServer.Domain.Devices;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.DeviceHeartbeats;
public sealed record DeviceHeartbeatReceivedCommand(
    Guid DeviceId,

    bool IsReachable,

    int? ResponseTimeMs,

    string? ErrorMessage
) : IRequest<ServiceResult>;
internal sealed class DeviceHeartbeatReceivedCommandHandler(
    IDeviceRepository deviceRepository,
    IDeviceHeartbeatRepository heartbeatRepository,
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<
        DeviceHeartbeatReceivedCommand,
        ServiceResult>
{
    public async Task<ServiceResult> Handle(
        DeviceHeartbeatReceivedCommand request,
        CancellationToken cancellationToken)
    {
        var device = await deviceRepository
            .FirstOrDefaultAsync(
                x => x.Id == request.DeviceId,
                cancellationToken);

        if (device is null)
        {
            return ServiceResult.Failure(
                "Tapılmadı",
                "Device tapılmadı",
                HttpStatusCode.NotFound);
        }

        var heartbeat = new DeviceHeartbeat(
            request.DeviceId,
            request.IsReachable
                ? DeviceStatus.Online
                : DeviceStatus.Offline,
            request.IsReachable,
            request.ResponseTimeMs,
            request.ErrorMessage);

        await heartbeatRepository.AddAsync(
            heartbeat,
            cancellationToken);

        var oldStatus = device.Status;

        if (request.IsReachable)
        {
            device.MarkHeartbeatSuccess(
                request.ResponseTimeMs);
        }
        else
        {
            device.MarkHeartbeatFailure();
        }

        if (oldStatus != device.Status)
        {
            var branch = await branchRepository
                .FirstOrDefaultAsync(
                    x => x.Id == device.BranchId,
                    cancellationToken);

            if (branch is not null)
            {
                branch.RecalculateDeviceStats();
            }
        }

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ServiceResult.Success();
    }
}