using NetWorkPassServer.Application.Branches;
using NetWorkPassServer.WEBAPI.Extensions;
using TS.MediatR;

namespace NetWorkPassServer.WEBAPI.Modules;

public static class BranchModule
{
    public static void MapBranch(this IEndpointRouteBuilder builder)
    {
        var group = builder
            .MapGroup("/branches")
            .WithTags("Branch")
            
            .RequireRateLimiting("fixed");

        // 🔥 CREATE
        group.MapPost("",
            async (BranchCreateCommand request, ISender sender, CancellationToken ct) =>
            {
                var res = await sender.Send(request, ct);
                return res.ToCreatedResult($"/branches/{res.Data}");
            }).RequireAuthorization();

        // 🔥 UPDATE
        group.MapPut("{id:guid}",
            async (Guid id, BranchUpdateCommand request, ISender sender, CancellationToken ct) =>
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
                var res = await sender.Send(new BranchDeleteCommand(id), ct);
                return res.ToNoContentResult();
            }).RequireAuthorization();

        // 🔥 GET ALL
        group.MapGet("",
            async (int page, int pageSize, ISender sender, CancellationToken ct) =>
            {
                var query = new BranchGetAllQuery(page, pageSize);
                var res = await sender.Send(query, ct);
                return res.ToResult();
            })
        .RequireAuthorization();

        // 🔥 GET BY ID
        group.MapGet("{id:guid}",
            async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var res = await sender.Send(new BranchGetQuery(id), ct);
                return res.ToResult();
            }).RequireAuthorization()
         ;


        group.MapGet("test-auth",
    (HttpContext context) =>
    {
        var isAuth = context.User.Identity?.IsAuthenticated;
        return Results.Ok(new
        {
            isAuth,
            claims = context.User.Claims.Select(x => new { x.Type, x.Value })
        });
    });
    }
}