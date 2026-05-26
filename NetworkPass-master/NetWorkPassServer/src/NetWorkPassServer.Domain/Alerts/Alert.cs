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
        AlertSource source,
        string message,
        string title,       
        DateTime triggeredAt,
         string fingerprint)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "Title boş ola bilməz");
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException(
                "Message boş ola bilməz");
        }
        if (string.IsNullOrWhiteSpace(fingerprint))
        {
            throw new ArgumentException(
                "Fingerprint boş ola bilməz");
        }




        DeviceId = deviceId;
        BranchId = branchId;
        Type = type;
        Title = title.Trim();
        Severity = severity;
        Source = source;
        Message = message;
        OccurrenceCount=1;
       Status = AlertStatus.Open;
        Fingerprint = fingerprint.Trim();
        TriggeredAt = triggeredAt;
    }

    // RELATIONS

    public Guid DeviceId { get; private set; }

    public Guid BranchId { get; private set; }

    public Device Device { get; private set; } = default!;

    public Branch Branch { get; private set; } = default!;

    // ALERT INFO

    public AlertType Type { get; private set; }

    public AlertSeverity Severity { get; private set; }
    public AlertSource Source { get; private set; }
    public string Title { get; private set; } = default!;
    public string Message { get; private set; } = default!;
    // 🔥 deduplication key
    public string Fingerprint { get; private set; } = default!;
    public Guid? ResolvedBy { get; private set; }
    // STATE
    public int OccurrenceCount { get; private set; }
    public AlertStatus Status { get; private set; }
    public DateTime TriggeredAt { get; private set; }
    public DateTime? AcknowledgedAt { get; private set; }
    public Guid? AcknowledgedBy { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    public string? ResolutionNote { get; private set; }

    // METHODS



    // METHODS
    public void IncrementOccurrence()
    {
        OccurrenceCount++;
    }
    public void Acknowledge(
        Guid userId)
    {
        if (Status == AlertStatus.Resolved)
        {
            return;
        }
        if (Status == AlertStatus.Acknowledged)
        {
            return;
        }

        Status = AlertStatus.Acknowledged;

        AcknowledgedAt = DateTime.UtcNow;

        AcknowledgedBy = userId;
    }



    public void Resolve(
      Guid? userId = null,
      string? resolutionNote = null)
    {
        if (Status == AlertStatus.Resolved)
        {
            return;
        }

        Status = AlertStatus.Resolved;

        ResolvedAt = DateTime.UtcNow;

        ResolutionNote = resolutionNote;

        ResolvedBy = userId;
    }
    public void ChangeMessage(
       string title,
       string message)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "Title boş ola bilməz");
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException(
                "Message boş ola bilməz");
        }

        Title = title.Trim();

        Message = message.Trim();
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