using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Abstractions;
public sealed class AuditUserInfoDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = default!;
    public string Email { get; init; } = default!;
}
