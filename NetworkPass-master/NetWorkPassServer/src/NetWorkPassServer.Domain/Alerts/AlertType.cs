using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Domain.Alerts
{
    public enum AlertType
    {


        DeviceOffline = 1,

        DeviceRecovered = 2,

        DeviceDegraded = 3,

        DeviceFlapping = 4,

        HighCpuUsage = 5,

        HighMemoryUsage = 6,

        HighTemperature = 7,

        HighPingLatency = 8,

        DevicePacketLoss = 9,

        VpnDisconnected = 10,

        VpnRecovered = 11,

        VpnDegraded = 12,

        VpnFlapping = 13,

        VpnPacketLoss = 14,

        InterfaceDown = 15
    }
}
