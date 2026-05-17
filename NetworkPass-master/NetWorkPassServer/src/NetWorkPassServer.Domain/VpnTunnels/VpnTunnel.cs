using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Domain.VpnTunnels;
public sealed class VpnTunnel : Entity
{
    public Guid BranchId { get; set; }

    public string TunnelName { get; set; }

    public string RemoteIpAddress { get; set; }

    public VpnStatus Status { get; set; }

    public DateTime? LastConnectedAt { get; set; }
}