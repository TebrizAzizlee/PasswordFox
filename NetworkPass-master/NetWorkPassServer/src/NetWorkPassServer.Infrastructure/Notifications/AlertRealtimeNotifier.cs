using Microsoft.AspNetCore.SignalR;
using NetWorkPassServer.Application.Dtos;
using NetWorkPassServer.Application.Notifications;
using NetWorkPassServer.Infrastructure.Hubs;


namespace NetWorkPassServer.Infrastructure.Notifications
{
    internal sealed class AlertRealtimeNotifier(
      IHubContext<AlertHub> hubContext)
      : IAlertRealtimeNotifier
    {
        public async Task AlertCreatedAsync(
            AlertListDto alert,
            CancellationToken cancellationToken)
        {
            await hubContext.Clients.All.SendAsync(
                "alert-created",
                alert,
                cancellationToken);
        }

        public async Task AlertResolvedAsync(
            Guid alertId,
            CancellationToken cancellationToken)
        {
            await hubContext.Clients.All.SendAsync(
                "alert-resolved",
                alertId,
                cancellationToken);
        }
    }
}
