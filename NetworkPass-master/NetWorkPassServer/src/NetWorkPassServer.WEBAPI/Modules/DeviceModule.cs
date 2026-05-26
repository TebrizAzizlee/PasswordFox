
using NetWorkPassServer.Application.Devices;
using NetWorkPassServer.Application.Dtos.DeviesDtos;
using NetWorkPassServer.WEBAPI.Extensions;
using TS.MediatR;

namespace NetWorkPassServer.WEBAPI.Modules;

public static class DeviceModule
{
    public static void MapDevice(this IEndpointRouteBuilder builder)
    {
        var group = builder
            .MapGroup("/devices")
            .WithTags("Device")
            
            .RequireRateLimiting("fixed");

        // 🔥 CREATE
        group.MapPost("",
            async (DeviceCreateCommand request, ISender sender, CancellationToken ct) =>
            {
                var res = await sender.Send(request, ct);
                return res.ToCreatedResult($"/devices/{res.Data}");
            }).RequireAuthorization();

        // 🔥 UPDATE
        group.MapPut("{id:guid}",
            async (Guid id, UpdateDeviceRequest request, ISender sender, CancellationToken ct) =>
            {
                var command =
     new DeviceUpdateCommand(
         id,
        request.Name,
        request.IpAddress,
        request.Type,
      request.Vendor,
      request.Role,
        request.Model,
        request.IsCritical,
         request.Description);
                var result =
       await sender.Send(command, ct);

                return result.ToNoContentResult();
            }).RequireAuthorization();

        // 🔥 DELETE
        group.MapDelete("{id:guid}",
            async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var res = await sender.Send(new DeviceDeleteCommand(id), ct);
                return res.ToNoContentResult();
            }).RequireAuthorization();

        // 🔥 GET ALL
        group.MapGet("",
            async (Guid? branchId, string? search, int? type, int page, int pageSize, ISender sender, CancellationToken ct) =>
            {
                var query = new DeviceGetAllQuery(branchId, search, type, page, pageSize);
                var res = await sender.Send(query, ct);
                return res.ToResult();
            })
        . RequireAuthorization(); // lazım olsa

        // 🔥 GET BY ID
        group.MapGet("{id:guid}",
            async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var res = await sender.Send(new DeviceGetByIdQuery(id), ct);
                return res.ToResult();
            })
        . RequireAuthorization();
    }
}