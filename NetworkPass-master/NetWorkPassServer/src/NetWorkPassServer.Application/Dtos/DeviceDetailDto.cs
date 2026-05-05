using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Dtos;
public sealed record DeviceDetailDto(
    Guid Id,
    Guid BranchId,
    string BranchName,
    string Name,
    string IpAddress,
    string Type,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAt
);