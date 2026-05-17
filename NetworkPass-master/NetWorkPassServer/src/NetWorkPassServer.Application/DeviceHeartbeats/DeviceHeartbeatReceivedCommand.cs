using SharedLibrary;

using TS.MediatR;

namespace NetWorkPassServer.Application.DeviceHeartbeats;
public sealed record DeviceHeartbeatReceivedCommand(
    Guid DeviceId,

    bool IsReachable,

    int? ResponseTimeMs,

    string? ErrorMessage
) : IRequest<ServiceResult>;
