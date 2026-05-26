using GenericRepository;
using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Domain.DeviceMetricss;
using NetWorkPassServer.Infrastructure.Context;

namespace NetWorkPassServer.Infrastructure.Repositories
{
    internal sealed class DeviceMetricRepository(
        PasswordDbContext context)
        : Repository<DeviceMetric, PasswordDbContext>(context),
      IDeviceMetricRepository
    {
        private readonly PasswordDbContext _context = context;

        public async Task<DeviceMetric?> GetLatestAsync(Guid deviceId, CancellationToken cancellationToken)
        {
            return await _context.DeviceMetrics
         .AsNoTracking()
         .Where(x => x.DeviceId == deviceId)
         .OrderByDescending(x => x.OccurredAtUtc)
         .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
