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

        Connecting = 2,

        Disconnected = 3,

        Error = 4
    }
}
