

namespace NetWorkPassServer.Application.Dtos.AlertsDtos;
public sealed record ResolveAlertRequest(
    Guid? ResolvedBy,
    string? ResolutionNote);