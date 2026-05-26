using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Dtos.BranchDtos;
using NetWorkPassServer.Domain.Alerts;
using SharedLibrary;
using SharedLibrary.Abstractions.Pagination;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record GetBranchAlertsQuery(
    Guid BranchId,
    AlertSeverity? Severity,
    AlertStatus? Status,
     int Page = 1,
    int PageSize = 20
   )
    : IRequest <ServiceResult<PagedResult<BranchAlertItemDto>>>;


internal sealed class GetBranchAlertsQueryHandler(
    IPasswordDbContext context)
    : IRequestHandler<
        GetBranchAlertsQuery,
         ServiceResult<
            PagedResult<BranchAlertItemDto>>>
{
    public async Task<
         ServiceResult<
            PagedResult<BranchAlertItemDto>>>
        Handle(
            GetBranchAlertsQuery request,
            CancellationToken cancellationToken)
    {

        var page =
           request.Page < 1
               ? 1
               : request.Page;

        var pageSize =
            request.PageSize switch
            {
                < 1 => 20,
                > 100 => 100,
                _ => request.PageSize
            };
        var branchExists = await context.Branches
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.Id == request.BranchId &&
                    !x.IsDeleted &&
                    x.IsActive,
                cancellationToken);

        if (!branchExists)
        {
            return ServiceResult<
                PagedResult<
                    BranchAlertItemDto>>
                .Failure(
                    "Tapılmadı",
                    "Şöbə tapılmadı",
                    HttpStatusCode.NotFound);
        }

        var query = context.Alerts
            .AsNoTracking()
            .Where(x =>!x.IsDeleted &&
                x.BranchId == request.BranchId);

        if (request.Severity.HasValue)
        {
            query = query.Where(x =>
                x.Severity == request.Severity.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x =>
                x.Status == request.Status.Value);
        }
        var totalCount =
          await query.CountAsync(
              cancellationToken);
        var alerts = await query
            .OrderByDescending(x =>
                x.TriggeredAt)
            .Select(x =>
                new BranchAlertItemDto(
                    x.Id,           
                    x.Title,
                    x.Message,
                    x.Type,
                    x.Severity,

                    x.Status,
                    x.OccurrenceCount,
                    x.TriggeredAt,
                    x.CreationTime
                ))
            .ToListAsync(cancellationToken);
        var result =
            new PagedResult<
                BranchAlertItemDto>(
                    alerts,
                    totalCount,
                    page,
                    pageSize);
        return ServiceResult<
            PagedResult<
                BranchAlertItemDto>>
            .Success(result);
    }
}