using NetWorkPassServer.Domain.Shared;
using NetWorkPassServer.Domain.VpnTunnels;
using System.Net;


namespace NetWorkPassServer.Application.Branches;
public sealed record BranchVpnTunnelItemDto(
    Guid Id,

    string TunnelName,

    IpAddress RemoteIpAddress,

    VpnStatus Status,

    DateTime? LastConnectedAt
);