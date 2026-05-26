using NetWorkPassServer.Application.DeviceHeartbeats;
using NetWorkPassServer.WEBAPI.Extensions;
using TS.MediatR;

namespace NetWorkPassServer.WEBAPI.Modules;

public static class DeviceHeartbeatModule
{
    public static void RegisterDeviceHeartbeatRoutes(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/device-heartbeats")
            .WithTags("DeviceHeartbeats");

        // 🔥 ingest heartbeat

        group.MapPost(
            "",
            async (
                DeviceHeartbeatRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command =
                    new DeviceHeartbeatReceivedCommand(

                        request.DeviceId,

                        request.IsReachable,

                        request.ErrorMessage,


                        request.CpuUsage,

                        request.DiskUsage,

                        request.MemoryUsage,

                        request.Temperature,

                        request.UptimeSeconds,

                        request.ResponseTimeMs
                    );

                var result =
                    await sender.Send(
                        command,
                        ct);

                return result.ToNoContentResult();
            });
    }
}