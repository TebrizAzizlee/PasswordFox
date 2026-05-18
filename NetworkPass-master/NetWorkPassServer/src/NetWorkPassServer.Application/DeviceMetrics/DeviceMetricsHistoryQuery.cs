using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.DeviceMetrics;
using NetWorkPassServer.Application.Dtos;
using SharedLibrary;

using TS.MediatR;

namespace NetWorkPassServer.Application.DeviceMetrics
{
    public sealed record DeviceMetricsHistoryQuery(
    Guid DeviceId,

    DateTime StartDate,

    DateTime EndDate
) : IRequest<ServiceResult<List<MetricPointDto>>>;
}


internal sealed class DeviceMetricsHistoryQueryHandler(
    IPasswordDbContext context)
    : IRequestHandler<
        DeviceMetricsHistoryQuery,
        ServiceResult<List<MetricPointDto>>>
{
    public async Task<ServiceResult<List<MetricPointDto>>> Handle(
        DeviceMetricsHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var metrics = await context.DeviceMetrics
            .AsNoTracking()
            .Where(x =>
                x.DeviceId == request.DeviceId &&
                x.Timestamp >= request.StartDate &&
                x.Timestamp <= request.EndDate)
            .OrderBy(x => x.Timestamp)
            .Select(x => new MetricPointDto(
                x.Timestamp,
                x.CpuUsage,
                x.MemoryUsage,
                x.Temperature,
                x.PingLatency
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<MetricPointDto>>
            .Success(metrics);
    }
}
