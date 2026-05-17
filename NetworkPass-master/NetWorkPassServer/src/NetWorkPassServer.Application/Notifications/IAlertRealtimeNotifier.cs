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

        Task AlertResolvedAsync(
            Guid alertId,
            CancellationToken cancellationToken);
    }
}
