using NetWorkPassServer.Application.Alerts;
using NetWorkPassServer.Domain.Devices;

namespace NetWorkPassServer.Application.Services
{
    public interface IAlertService
    {
        Task ProcessAsync(
    AlertContext context,
    CancellationToken cancellationToken);

        Task ResolveAsync(
    string fingerprint,
    CancellationToken cancellationToken);
        //    Task HandleDeviceOfflineAsync(
        //   Device device,
        //   CancellationToken cancellationToken);

        //    Task HandleDeviceRecoveredAsync(
        //        Device device,
        //        CancellationToken cancellationToken);

        //    Task HandleHighCpuUsageAsync(
        //        Device device,
        //        double cpuUsage,
        //        CancellationToken cancellationToken);

        //    Task HandleHighMemoryUsageAsync(
        //        Device device,
        //        double memoryUsage,
        //        CancellationToken cancellationToken);

        //    Task HandleHighLatencyAsync(
        //        Device device,
        //        long latency,
        //        CancellationToken cancellationToken);
    }
}
