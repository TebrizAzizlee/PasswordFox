using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Context;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record GetBranchVpnTunnelsQuery(
    Guid BranchId)
    : IRequest<ServiceResult<List<BranchVpnTunnelItemDto>>>;
internal sealed class GetBranchVpnTunnelsQueryHandler(
    IPasswordDbContext context)
    : IRequestHandler<
        GetBranchVpnTunnelsQuery,
        ServiceResult<List<BranchVpnTunnelItemDto>>>
{
    public async Task<
        ServiceResult<List<BranchVpnTunnelItemDto>>>
        Handle(
            GetBranchVpnTunnelsQuery request,
            CancellationToken cancellationToken)
    {
        var branchExists = await context.Branches
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.Id == request.BranchId &&
                    x.IsActive,
                cancellationToken);

        if (!branchExists)
        {
            return ServiceResult<
                List<BranchVpnTunnelItemDto>>
                .Failure(
                    "Tapılmadı",
                    "Şöbə tapılmadı",
                    HttpStatusCode.NotFound);
        }

        var tunnels = await context.VpnTunnels
            .AsNoTracking()
            .Where(x =>
                x.BranchId == request.BranchId)
            .OrderBy(x =>
                x.TunnelName)
            .Select(x =>
                new BranchVpnTunnelItemDto(
                    x.Id,

                    x.TunnelName,
                    x.RemoteIpAddress
                    ,

                    x.Status,

                    x.LastConnectedAt
                ))
            .ToListAsync(cancellationToken);

        return ServiceResult<
            List<BranchVpnTunnelItemDto>>
            .Success(tunnels);
    }
}