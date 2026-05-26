
namespace NetWorkPassServer.Application.Services;
public interface IDashboardNotifier
{
    Task NotifyDashboardUpdatedAsync(
        CancellationToken cancellationToken);
}