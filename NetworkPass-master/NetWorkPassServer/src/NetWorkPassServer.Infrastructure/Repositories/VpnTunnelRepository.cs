using GenericRepository;
using NetWorkPassServer.Domain.VpnTunnels;
using NetWorkPassServer.Infrastructure.Context;


namespace NetWorkPassServer.Infrastructure.Repositories;
internal sealed class VpnTunnelRepository(
    PasswordDbContext context)
        : Repository<VpnTunnel, PasswordDbContext>(context),
      IVpnTunnelRepository
{
}
