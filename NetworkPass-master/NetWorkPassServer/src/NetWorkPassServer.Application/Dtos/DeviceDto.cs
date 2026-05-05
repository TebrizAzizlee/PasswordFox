using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Dtos;
public sealed record DeviceDto(Guid Id,
    Guid BranchId,
    string Name,
    string IpAddress,
    int Type,
    string? Description,
    bool IsActive);

