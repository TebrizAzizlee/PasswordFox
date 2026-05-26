
using NetWorkPassServer.Domain.Devices;

namespace NetWorkPassServer.Application.Dtos.DeviesDtos;
public sealed record UpdateDeviceRequest
(
    
    string Name,
    string IpAddress,
    DeviceType Type,
    string Vendor,
    DeviceRole Role,
    string Model,
    bool IsCritical,
    string? Description
);
