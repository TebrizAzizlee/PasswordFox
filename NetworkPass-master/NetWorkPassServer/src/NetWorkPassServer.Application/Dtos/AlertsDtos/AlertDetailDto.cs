using NetWorkPassServer.Domain.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Dtos.AlertsDtos;
public sealed  record AlertDetailDto(Guid Id,

    Guid DeviceId,

    Guid BranchId,

    string DeviceName,

    string BranchName,

    AlertType Type,

    AlertSeverity Severity,

    AlertStatus Status,

    AlertSource Source,

    string Title,

    string Message,

    string Fingerprint,

    int OccurrenceCount,

    DateTime TriggeredAt,

    DateTime? AcknowledgedAt,

    Guid? AcknowledgedBy,

    DateTime? ResolvedAt,

    Guid? ResolvedBy,

    string? ResolutionNote);

