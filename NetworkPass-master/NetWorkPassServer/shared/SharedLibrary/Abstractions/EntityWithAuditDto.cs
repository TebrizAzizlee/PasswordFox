using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Abstractions;
public sealed class EntityWithAuditDto<TDto>
{
    public TDto Entity { get; set; } = default!;
    public AuditUserInfoDto CreatedUser { get; set; } = default!;
    public AuditUserInfoDto? UpdatedUser { get; set; }
}
