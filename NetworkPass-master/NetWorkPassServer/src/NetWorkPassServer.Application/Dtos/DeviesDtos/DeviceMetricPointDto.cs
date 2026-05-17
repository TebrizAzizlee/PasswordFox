namespace NetWorkPassServer.Application.Dtos.DeviesDtos;
public sealed record DeviceMetricPointDto(
    DateTime Timestamp,

    double? CpuUsage,

    double? MemoryUsage,

    double? Temperature,

    double? PingLatency
);