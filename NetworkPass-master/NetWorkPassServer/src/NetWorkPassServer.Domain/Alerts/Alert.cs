using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Domain.Alerts;
public sealed class Alert : FullAuditedAggregateRoot
{
    public Guid? BranchId { get; set; }

    public Guid? DeviceId { get; set; }

    public AlertSeverity Severity { get; set; }

    public AlertType Type { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public bool IsResolved { get; set; }

    public DateTime? ResolvedAt { get; set; }
}