using GenericRepository;
using NetWorkPassServer.Domain.DeviceMetricss;
using NetWorkPassServer.Infrastructure.Context;


namespace NetWorkPassServer.Infrastructure.Repositories
{
    internal sealed class DeviceMetricRepository
    : Repository<DeviceMetric, PasswordDbContext>,
      IDeviceMetricRepository
    {
        public DeviceMetricRepository(
            PasswordDbContext context)
            : base(context)
        {
        }
    }
}
