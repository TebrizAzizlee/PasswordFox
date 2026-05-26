using NetWorkPassServer.Application.Alerts;
using NetWorkPassServer.Application.Dtos.AlertsDtos;
using TS.MediatR;
using NetWorkPassServer.WEBAPI.Extensions;
namespace NetWorkPassServer.WEBAPI.Modules;

public static class AlertModule
{
    public static void RegisterAlertRoutes(
       this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/alerts")
            .WithTags("Alerts");

        // =====================================================
        // GET ALL ALERTS
        // =====================================================
        group.MapGet(
            "",
            async (
                int page,
                int pageSize,
                ISender sender,
                CancellationToken ct) =>
            {
                var result =
                    await sender.Send(
                        new GetAlertsQuery(
                            page,
                            pageSize),
                        ct);

                return result.ToResult();
            })
            .RequireAuthorization();

        // =====================================================
        // GET ACTIVE ALERTS
        // =====================================================

        group.MapGet(
            "active",
            async (
                ISender sender,
                CancellationToken ct) =>
            {
                var result =
                    await sender.Send(
                        new GetActiveAlertsQuery(),
                        ct);

                return result.ToResult();
            })
            .RequireAuthorization();

        // =====================================================
        // GET CRITICAL ALERTS
        // =====================================================

        group.MapGet(
            "critical",
            async (
                ISender sender,
                CancellationToken ct) =>
            {
                var result =
                    await sender.Send(
                        new GetCriticalAlertsQuery(),
                        ct);

                return result.ToResult();
            })
            .RequireAuthorization();

        // =====================================================
        // GET ALERT DETAIL
        // =====================================================

        group.MapGet(
            "{id:guid}",
            async (
                Guid id,
                ISender sender,
                CancellationToken ct) =>
            {
                var result =
                    await sender.Send(
                        new GetAlertByIdQuery(id),
                        ct);

                return result.ToResult();
            })
            .RequireAuthorization();

        // =====================================================
        // ACKNOWLEDGE ALERT
        // =====================================================

        group.MapPost(
            "{id:guid}/acknowledge",
            async (
                Guid id,
                AcknowledgeAlertRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command =
                    new AcknowledgeAlertCommand(
                        id,
                        request.UserId);

                var result =
                    await sender.Send(
                        command,
                        ct);

                return result.ToNoContentResult();
            })
            .RequireAuthorization();

        // =====================================================
        // RESOLVE ALERT
        // =====================================================

        group.MapPost(
            "{id:guid}/resolve",
            async (
                Guid id,
                ResolveAlertRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command =
                    new ResolveAlertCommand(
                        id,
                        request.ResolvedBy,
                        request.ResolutionNote);

                var result =
                    await sender.Send(
                        command,
                        ct);

                return result.ToNoContentResult();
            })
            .RequireAuthorization();

        // =====================================================
        // ALERT DASHBOARD
        // =====================================================

        group.MapGet(
            "dashboard",
            async (
                ISender sender,
                CancellationToken ct) =>
            {
                var result =
                    await sender.Send(
                        new GetAlertDashboardQuery(),
                        ct);

                return result.ToResult();
            })
            .RequireAuthorization();
    }
}
