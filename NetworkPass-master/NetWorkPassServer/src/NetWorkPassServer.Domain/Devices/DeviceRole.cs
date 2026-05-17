using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Domain.Devices;
public enum DeviceRole
{
    CoreRouter,
    EdgeRouter,
    DistributionSwitch,
    AccessSwitch,
    Firewall,
    WirelessController,
    AccessPoint
}
