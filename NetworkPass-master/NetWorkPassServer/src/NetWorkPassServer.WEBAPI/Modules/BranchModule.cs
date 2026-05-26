using NetWorkPassServer.Application.Branches;
using NetWorkPassServer.Application.Dtos.BranchDtos;
using NetWorkPassServer.Domain.Branches;
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
            async (Guid id, UpdateBranchRequest request, ISender sender, CancellationToken ct) =>
            {
                var command =
       new BranchUpdateCommand(
           id,
           request.Code,
           request.BranchName,
           request.City,
           request.District,
           request.FullAddress,
           request.PhoneNumber1,
           request.PhoneNumber2,
           request.Email,
           request.WanIp,
           request.Subnet,
           request.Gateway,
           request.DnsServer,
           request.Type,
           request.Description);
                var result =
        await sender.Send(command, ct);

                return result.ToNoContentResult();
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
      async (
          ISender sender,
          CancellationToken ct,
          string? search = null,
          BranchStatus? status = null,
          BranchType? type = null,
          int page = 1,
          int pageSize = 10) =>
      {
          page = page < 1
              ? 1
              : page;

          pageSize = pageSize switch
          {
              < 1 => 10,
              > 100 => 100,
              _ => pageSize
          };

          var query = new BranchGetAllQuery(
              search,
              status,
              type,
              page,
              pageSize);

          var res = await sender.Send(query, ct);

          return res.ToResult();
      })
  .RequireAuthorization();

        // 🔥 GET BY ID
        group.MapGet("{id:guid}",
            async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var res = await sender.Send(new GetBranchDetailsQuery(id), ct);
                return res.ToResult();
            }).RequireAuthorization()
         ;

        group.MapGet(
               "monitoring",
    async (
        ISender sender,
        CancellationToken ct) =>
    {
        var result = await sender.Send(
            new BranchMonitoringListQuery(),
            ct);

        return result.ToResult();
    })
.RequireAuthorization();

        group.MapGet(
            "monitoring/{id:guid}",
            async (
                Guid id,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new BranchMonitoringDetailsQuery(id),
                    ct);

                return result.ToResult();
            })
        .RequireAuthorization();

    }
}