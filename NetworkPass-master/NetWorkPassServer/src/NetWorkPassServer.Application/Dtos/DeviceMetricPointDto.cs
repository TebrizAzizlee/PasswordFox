

namespace NetWorkPassServer.Application.Dtos;
public sealed record DeviceMetricPointDto(
    DateTime Timestamp,

    double? CpuUsage,

    double? MemoryUsage,

    double? Temperature,

    double? PingLatency
);