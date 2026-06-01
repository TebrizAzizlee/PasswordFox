using NetWorkPassServer.Application.Dashboard;
using NetWorkPassServer.WEBAPI.Extensions;
using TS.MediatR;

namespace NetWorkPassServer.WEBAPI.Modules;

public static class DashboardModule
{
    public static void RegisterDashboardRoutes(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/dashboard")
            .WithTags("Dashboard");

        // =====================================================
        // DASHBOARD SUMMARY
        // =====================================================

        group.MapGet(
            "summary",
            async (
                ISender sender,
                CancellationToken ct) =>
            {
                var result =
                    await sender.Send(
                        new DashboardSummaryQuery(),
                        ct);

                return result.ToResult();
            })
            .RequireAuthorization();
    }
}
