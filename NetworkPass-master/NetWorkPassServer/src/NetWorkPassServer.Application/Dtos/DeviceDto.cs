using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetWorkPassServer.Domain.Devices.Device;

namespace NetWorkPassServer.Application.Dtos;
public sealed record DeviceDto(Guid Id,
    Guid BranchId,
    string Name,
    string IpAddress,
    DeviceType? Type,
    string? Description,
    bool IsActive);

