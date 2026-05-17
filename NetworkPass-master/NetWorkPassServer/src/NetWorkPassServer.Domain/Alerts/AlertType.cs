using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Domain.Alerts
{
    public enum AlertType
    {
        Unknown = 0,

        DeviceOffline = 1,

        HighCpuUsage = 2,

        HighMemoryUsage = 3,

        HighTemperature = 4,

        HighPingLatency = 5,

        PacketLoss = 6,

        VpnDisconnected = 7,

        InterfaceDown = 8
    }
}
