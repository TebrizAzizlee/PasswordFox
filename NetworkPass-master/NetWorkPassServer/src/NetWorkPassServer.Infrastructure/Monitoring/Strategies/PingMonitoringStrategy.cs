using NetWorkPassServer.Application.Monitoring;
using NetWorkPassServer.Domain.Devices;
using System.Net.NetworkInformation;

namespace NetWorkPassServer.Infrastructure.Monitoring.Strategies;
internal sealed class PingMonitoringStrategy
    : IDeviceMonitoringStrategy
{
    public bool CanHandle(DeviceType deviceType)

    {
        return true;
    }

    public async Task<MonitoringResult> MonitorAsync(string ipAddress, CancellationToken cancellationToken)

    {
        try
        {
            using var ping =
                new Ping();

            var reply =
                await ping.SendPingAsync(
                    ipAddress,
                    3000);

            var isReachable =
                reply.Status ==
                IPStatus.Success;

            return new MonitoringResult
            {
                IsReachable = isReachable,

                ResponseTimeMs =
                    isReachable
                        ? reply.RoundtripTime
                        : null,

                ErrorMessage =
                    isReachable
                        ? null
                        : reply.Status.ToString(),

                Status =
                    isReachable
                        ? DeviceStatus.Online
                        : DeviceStatus.Offline
            };
        }
        catch (Exception ex)
        {
            return new MonitoringResult
            {
                IsReachable = false,

                ErrorMessage = ex.Message,

                Status = DeviceStatus.Offline
            };
        }
    }
}
