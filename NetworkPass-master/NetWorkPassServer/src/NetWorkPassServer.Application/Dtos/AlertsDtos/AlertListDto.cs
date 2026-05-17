using NetWorkPassServer.Domain.Alerts;

namespace NetWorkPassServer.Application.Dtos;

public sealed record AlertListDto(
    Guid Id,

    Guid DeviceId,

    Guid BranchId,

    string DeviceName,

    string BranchName,

    AlertType Type,

    AlertSeverity Severity,

    string Message,

    bool IsResolved,

    DateTime TriggeredAt,

    DateTime? ResolvedAt
);