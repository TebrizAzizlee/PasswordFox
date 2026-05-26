using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Domain.VpnTunnels
{
    public enum VpnStatus
    {
        Unknown = 0,

        Connected = 1,

        Degraded = 2,

        Disconnected = 3,

        Maintenance = 4
    }
}
