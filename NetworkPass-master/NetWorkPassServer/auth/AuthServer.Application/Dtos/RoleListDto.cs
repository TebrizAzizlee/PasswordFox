using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Application.Dtos;
public sealed record RoleListDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    int UserCount);