using NetWorkPassServer.Domain.Devices;


namespace NetWorkPassServer.Application.Monitoring;
public interface IDeviceMonitoringStrategy
{
    bool CanHandle(DeviceType deviceType);
    Task<MonitoringResult> MonitorAsync(string ipAddress, CancellationToken cancellationToken);


}