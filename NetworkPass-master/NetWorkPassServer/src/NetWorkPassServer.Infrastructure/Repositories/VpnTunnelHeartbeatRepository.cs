
using GenericRepository;
using NetWorkPassServer.Domain.VpnTunnelHeartbeats;
using NetWorkPassServer.Infrastructure.Context;

namespace NetWorkPassServer.Infrastructure.Repositories;
internal sealed class VpnTunnelHeartbeatRepository(
    PasswordDbContext context)
        : Repository<VpnTunnelHeartbeat, PasswordDbContext>(context),
        IVpnTunnelHeartbeatRepository
{
}
