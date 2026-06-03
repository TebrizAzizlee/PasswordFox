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
            Console.WriteLine(
       $"ALERT CREATED SENT: {alert.Id}");
            await hubContext.Clients.All.SendAsync(
                "alert-created",
                alert,
                cancellationToken);

            // 🔥 branch specific stream
            await hubContext.Clients.Group($"branch:{alert.BranchId}").SendAsync("alerts:created",
                alert,
                cancellationToken);

        }

        public async Task AlertAcknowledgedAsync(
            Guid alertId,
            Guid acknowledgedBy,
            DateTime acknowledgedAt,
            CancellationToken cancellationToken)
        {
            var payload = new
            {
                AlertId = alertId,

                AcknowledgedBy = acknowledgedBy,

                AcknowledgedAt = acknowledgedAt
            };
            await hubContext.Clients
             .Group("noc")
             .SendAsync(
                 "alerts:acknowledged",
                 payload,
                 cancellationToken);
        }

        public async Task AlertResolvedAsync(
      Guid alertId,
      Guid? resolvedBy,
      DateTime resolvedAt,
      string? resolutionNote,
      CancellationToken cancellationToken)
        {
            var payload = new
            {
                AlertId = alertId,

                ResolvedBy = resolvedBy,

                ResolvedAt = resolvedAt,


                ResolutionNote = resolutionNote
            };

            await hubContext.Clients
                .Group("noc")
                .SendAsync(
                    "alerts:resolved",
                    payload,
                    cancellationToken);
        }
    }
}


