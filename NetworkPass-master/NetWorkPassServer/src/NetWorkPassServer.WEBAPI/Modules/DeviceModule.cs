using NetWorkPassServer.Application.Devices;
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
            async (Guid id, DeviceUpdateCommand request, ISender sender, CancellationToken ct) =>
            {
                if (request.Id != Guid.Empty && request.Id != id)
                    return Results.BadRequest("Id mismatch");

                request = request with { Id = id };

                var res = await sender.Send(request, ct);
                return res.ToNoContentResult();
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