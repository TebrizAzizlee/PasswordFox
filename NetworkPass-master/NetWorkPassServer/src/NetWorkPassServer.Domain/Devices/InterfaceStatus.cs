
namespace NetWorkPassServer.Domain.Devices;
public enum InterfaceStatus
{
    Unknown = 0,

    Up = 1,

    Down = 2,

    Warning = 3,

    Testing = 4,

    Dormant = 5,

    NotPresent = 6,

    LowerLayerDown = 7,

    AdministrativelyDown = 8
}