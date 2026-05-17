using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Services;
using NetWorkPassServer.Domain.Alerts;


namespace NetWorkPassServer.Infrastructure.Services;

internal sealed class AlertService(
    IAlertRepository alertRepository)
    : IAlertService
{
    public async Task HandleDeviceOfflineAsync(
        Device device,
        CancellationToken cancellationToken)
    {
        var existingAlert =
            await alertRepository
                .Where(x =>
                    x.DeviceId == device.Id &&
                    x.Type == AlertType.DeviceOffline &&
                    !x.IsResolved)
                .FirstOrDefaultAsync(cancellationToken);

        if (existingAlert is not null)
        {
            return;
        }

        var alert = new Alert(
            device.Id,
            device.BranchId,
            AlertType.DeviceOffline,
            AlertSeverity.Critical,
            $"{device.Name.Value} cihazı offline oldu");

        await alertRepository.AddAsync(
            alert,
            cancellationToken);
    }

    public async Task HandleDeviceRecoveredAsync(
        Device device,
        CancellationToken cancellationToken)
    {
        var activeAlert =
            await alertRepository
                .Where(x =>
                    x.DeviceId == device.Id &&
                    x.Type == AlertType.DeviceOffline &&
                    !x.IsResolved)
                .FirstOrDefaultAsync(cancellationToken);

        if (activeAlert is null)
        {
            return;
        }

        activeAlert.Resolve(
            "Cihaz yenidən online oldu");
    }
}
