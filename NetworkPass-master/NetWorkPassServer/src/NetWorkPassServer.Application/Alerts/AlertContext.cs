

using NetWorkPassServer.Domain.Alerts;
using NetWorkPassServer.Domain.Devices;

namespace NetWorkPassServer.Application.Alerts;
public sealed class AlertContext
{
   
    public Device Device { get; init; } = default!;

    public AlertType Type { get; init; }

    public AlertSeverity Severity { get; init; }
    public AlertSource Source { get; init; }
    public string Title { get; init; } = default!;
    public string Message { get; init; } = default!;
   

    public string Fingerprint { get; init; } = default!;
}