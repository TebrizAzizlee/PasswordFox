using NetWorkPassServer.Domain.Devices;



namespace NetWorkPassServer.Application.Dtos.DeviesDtos;
public sealed record DeviceDto(Guid Id,
    Guid BranchId,
    string Name,
    string IpAddress,
    DeviceType? Type,
    string? Description,
    bool IsActive);

