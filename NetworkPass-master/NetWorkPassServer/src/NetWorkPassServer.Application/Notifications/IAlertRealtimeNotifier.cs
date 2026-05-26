using NetWorkPassServer.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Notifications
{
    public interface IAlertRealtimeNotifier
    {
        Task AlertCreatedAsync(
         AlertListDto alert,
         CancellationToken cancellationToken);

        Task AlertAcknowledgedAsync(
            Guid alertId,
            Guid acknowledgedBy,
            DateTime acknowledgedAt,
            CancellationToken cancellationToken);

        Task AlertResolvedAsync(
            Guid alertId,
            Guid? resolvedBy,
            DateTime resolvedAt,
            string? resolutionNote,
            CancellationToken cancellationToken);
    }
}
