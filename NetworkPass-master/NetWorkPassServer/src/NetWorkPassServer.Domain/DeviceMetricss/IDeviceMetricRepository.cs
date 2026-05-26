using GenericRepository;



namespace NetWorkPassServer.Domain.DeviceMetricss
{
    public interface IDeviceMetricRepository
     : IRepository<DeviceMetric>
    {
        Task<DeviceMetric?> GetLatestAsync(
    Guid deviceId,
    CancellationToken cancellationToken);


    }
}
