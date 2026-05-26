using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Domain.DeviceInterfaces;
public enum InterfaceType
{
    Unknown = 0,

    Ethernet = 1,

    Fiber = 2,

    Wireless = 3,

    Loopback = 4,

    Tunnel = 5,

    Vlan = 6,

    Bridge = 7,

    Bonding = 8,

    Virtual = 9,

    Wan = 10,

    Lan = 11,

    Management = 12
}
