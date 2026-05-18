using Microsoft.AspNetCore.SignalR;
using NetWorkPassServer.Application.Dtos;
using NetWorkPassServer.Application.Services;
using NetWorkPassServer.Domain.DeviceMetricss;
using NetWorkPassServer.Infrastructure.Hubs;


namespace NetWorkPassServer.Infrastructure.Services
{
    internal sealed class DeviceMetricCollectorService(
    IHubContext<MonitoringHub> hubContext,
    IDeviceMetricRepository metricRepository)
    : IDeviceMetricCollectorService
    {
        public async Task ProcessAsync(
            LiveMetricDto metric,
            CancellationToken cancellationToken)
        {
            // realtime stream

            await hubContext.Clients.All.SendAsync(
                "metric-updated",
                metric,
                cancellationToken);

            // optional persistence

            var entity = new DeviceMetric(
                metric.DeviceId,
                metric.Timestamp,
                metric.CpuUsage,
                metric.MemoryUsage,
                metric.Temperature,
                metric.PingLatency,
                metric.Timestamp);

            await metricRepository.AddAsync(
                entity,
                cancellationToken);
        }
    }
}
