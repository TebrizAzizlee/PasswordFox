

using FluentValidation;
using NetWorkPassServer.Domain.Alerts;

namespace NetWorkPassServer.Application.Dtos.BranchDtos;



public sealed record BranchAlertItemDto(
    Guid Id,

    string Title,

    string Message,

    AlertType Type,

    AlertSeverity Severity,

    AlertStatus Status,

    int OccurrenceCount,

    DateTime TriggeredAt,

    DateTime? ResolvedAt
);
