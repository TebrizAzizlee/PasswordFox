using NetWorkPassServer.Application.Monitoring;
using NetWorkPassServer.Domain.Devices;


namespace NetWorkPassServer.Infrastructure.Monitoring;
internal sealed class DeviceMonitoringStrategyResolver(
    IEnumerable<IDeviceMonitoringStrategy> strategies)
    : IDeviceMonitoringStrategyResolver
{
    private readonly IEnumerable<IDeviceMonitoringStrategy>
        _strategies = strategies;

    public IDeviceMonitoringStrategy Resolve(
        DeviceType deviceType)
    {
        var strategy =
            _strategies.FirstOrDefault(
                x => x.CanHandle(deviceType));

        if (strategy is null)
        {
            throw new InvalidOperationException(
                $"No monitoring strategy found for device type: {deviceType}");
        }

        return strategy;
    }
}
