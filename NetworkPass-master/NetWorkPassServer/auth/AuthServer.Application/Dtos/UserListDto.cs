using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Application.Dtos;
public sealed record UserListDto(
    Guid Id,
    string FullName,
    string UserName,
    string Email,
    bool IsActive,
    List<string> UserRoles);