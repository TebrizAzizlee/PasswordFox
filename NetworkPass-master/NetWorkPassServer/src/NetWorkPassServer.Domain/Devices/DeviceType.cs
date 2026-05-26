using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Domain.Devices;
public enum DeviceType
{
    Router = 1,
    Switch = 2,
    AccessPoint = 3,
    Firewall = 4,
    Server = 5,
    Printer = 6,
    Camera = 7,
    FingerPrint=8
}