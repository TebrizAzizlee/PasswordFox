using Microsoft.AspNetCore.SignalR;
using NetWorkPassServer.Application.Services;
using NetWorkPassServer.Infrastructure.Hubs;

namespace NetWorkPassServer.Infrastructure.Services.Dashboard;
internal sealed class DashboardNotifier(
    IHubContext<DashboardHub> hubContext)
    : IDashboardNotifier
{
    public async Task NotifyDashboardUpdatedAsync(
        CancellationToken cancellationToken)
    {
        await hubContext.Clients.All.SendAsync(
            "dashboard-updated",
            cancellationToken);
    }
}