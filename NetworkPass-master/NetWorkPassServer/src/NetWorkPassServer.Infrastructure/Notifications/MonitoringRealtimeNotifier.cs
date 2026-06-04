using Microsoft.AspNetCore.SignalR;
using NetWorkPassServer.Application.Dtos.DeviesDtos;
using NetWorkPassServer.Application.Notifications;
using NetWorkPassServer.Infrastructure.Hubs;


namespace NetWorkPassServer.Infrastructure.Notifications;
internal sealed class MonitoringRealtimeNotifier(IHubContext<MonitoringHub> hubContext) : IMonitoringRealtimeNotifier
{
    public async Task DeviceStatusChangedAsync(DeviceRealtimeDto dto, CancellationToken cancellationToken)
    {
       await hubContext.Clients.All.SendAsync("device-updated",dto,cancellationToken);
    }
}
