using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Domain.Devices;
public enum DeviceRole
{
    Core = 1,
    Distribution = 2,
    Access = 3,
    Edge = 4,
    WAN = 5,
    VPN = 6,
    DMZ = 7
}
