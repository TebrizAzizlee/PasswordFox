using NetWorkPassServer.Domain.VpnTunnels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Services;
public interface IVpnAlertService
{
    Task HandleVpnStateChangedAsync(
        VpnTunnel vpnTunnel,
        CancellationToken cancellationToken);
}
