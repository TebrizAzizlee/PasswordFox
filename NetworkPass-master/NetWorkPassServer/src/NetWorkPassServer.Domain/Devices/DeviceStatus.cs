

namespace NetWorkPassServer.Domain.Devices;
public enum DeviceStatus
{
    //Hələ check olunmayıb
    Unknown = 0,
    //Yeni əlavə olunur
    Provisioning = 1,
    ///Normal işləyir
    Online = 2,
  
    //Device down
    Offline = 3,
  
    ///Bilərəkdən disable edilib
    Maintenance = 4,
  
    //Qismən problem var
    Degraded = 5
}
