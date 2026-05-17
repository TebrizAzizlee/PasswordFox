using Abp.Domain.Entities.Auditing;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Devices;

namespace NetWorkPassServer.Domain.Alerts;

public sealed class Alert : FullAuditedAggregateRoot<Guid>
{
    private Alert()
    {
    }

    public Alert(
        Guid deviceId,
        Guid branchId,
        AlertType type,
        AlertSeverity severity,
        string message)
    {
        DeviceId = deviceId;
        BranchId = branchId;
        Type = type;
        Severity = severity;
        Message = message;

        IsResolved = false;

        TriggeredAt = DateTime.UtcNow;
    }

    // RELATIONS

    public Guid DeviceId { get; private set; }

    public Guid BranchId { get; private set; }

    public Device Device { get; private set; } = default!;

    public Branch Branch { get; private set; } = default!;

    // ALERT INFO

    public AlertType Type { get; private set; }

    public AlertSeverity Severity { get; private set; }

    public string Message { get; private set; } = default!;

    // STATE

    public bool IsResolved { get; private set; }

    public DateTime TriggeredAt { get; private set; }

    public DateTime? ResolvedAt { get; private set; }

    public string? ResolutionNote { get; private set; }

    // METHODS

    public void Resolve(
        string? resolutionNote = null)
    {
        if (IsResolved)
        {
            return;
        }

        IsResolved = true;

        ResolvedAt = DateTime.UtcNow;

        ResolutionNote = resolutionNote;
    }

    public void ReOpen()
    {
        IsResolved = false;

        ResolvedAt = null;

        ResolutionNote = null;
    }

    public void ChangeSeverity(
        AlertSeverity severity)
    {
        Severity = severity;
    }

    public void ChangeMessage(
        string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException(
                "Message boş ola bilməz");
        }

        Message = message.Trim();
    }
}