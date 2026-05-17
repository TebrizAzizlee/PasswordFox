

using NetWorkPassServer.Application.Dtos;

namespace NetWorkPassServer.Application.Services
{
    public interface IDeviceMetricCollectorService
    {
        Task ProcessAsync(
            LiveMetricDto metric,
            CancellationToken cancellationToken);
    }
}
