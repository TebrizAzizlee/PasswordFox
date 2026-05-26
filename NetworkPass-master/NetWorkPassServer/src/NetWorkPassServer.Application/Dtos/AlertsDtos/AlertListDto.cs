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
    AlertStatus Status,
    string Message,

    DateTime TriggeredAt,

    DateTime? ResolvedAt
);