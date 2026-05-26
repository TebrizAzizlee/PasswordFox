using NetWorkPassServer.Domain.Devices;

namespace NetWorkPassServer.Application.Monitoring;
public interface IDeviceMonitoringStrategyResolver
{
    IDeviceMonitoringStrategy Resolve(DeviceType deviceType);

}
