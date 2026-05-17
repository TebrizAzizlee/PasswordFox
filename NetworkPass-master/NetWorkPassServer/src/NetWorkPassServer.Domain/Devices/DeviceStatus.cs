using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Domain.Devices;
public enum DeviceStatus
{
    //Hələ check olunmayıb
    Unknown = 0,
    ///Normal işləyir
    Online = 1,
    //Problem var amma işləyir
    Warning = 2,
    //Device down
    Offline = 3,
    //Network access yoxdur
    Unreachable = 4,
    ///Bilərəkdən disable edilib
    Maintenance = 5,
    //Yeni əlavə olunur
    Provisioning = 6,
    //Qismən problem var
    Degraded = 7
}
