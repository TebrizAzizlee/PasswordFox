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

            var entity = DeviceMetric.Create(
                metric.DeviceId,
                
                metric.Timestamp,
                metric.CpuUsage,
                metric.MemoryUsage,
                metric.DiskUsage,
                metric.Temperature,
                metric.UptimeSeconds,
                metric.PingLatency
                );

            await metricRepository.AddAsync(
                entity,
                cancellationToken);
        }
    }
}
