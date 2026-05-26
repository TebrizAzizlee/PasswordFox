using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Dtos.BranchDtos;
using NetWorkPassServer.Domain.Branches;
using SharedLibrary;
using SharedLibrary.Abstractions.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record BranchGetAllQuery(
    string? Search, 
    BranchStatus? Status,
    BranchType? Type,
    int Page = 1,
    int PageSize = 10
) : IRequest<ServiceResult<PagedResult<BranchListDto>>>;


internal sealed class BranchGetAllQueryHandler(
IBranchRepository branchRepository
) : IRequestHandler<BranchGetAllQuery, ServiceResult<PagedResult<BranchListDto>>>
{
   
    public async Task<ServiceResult<PagedResult<BranchListDto>>> Handle(
        BranchGetAllQuery request,
        CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 10 : request.PageSize;
        var query = branchRepository.Where(x=>!x.IsDeleted).AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                EF.Functions.Like(
                    x.Name.Value,
                    $"%{search}%") ||
                    
                    EF.Functions.Like(
                    x.Code, $"%{search}%"));
                
        }
        if (request.Status.HasValue)
        {
            query = query.Where(
                x => x.Status == request.Status.Value);
        }
        if (request.Type.HasValue)
        {
            query = query.Where(
                x => x.Type == request.Type.Value);
        }
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Name.Value) // default sorting
            .Skip((page-1)*pageSize)
            .Take(pageSize)
            .Select(x => new BranchListDto(
                x.Id,
                x.Code,
                x.Name.Value,
               x.Address.City,
               x.Type,
                x.Status,
                x.TotalDeviceCount,
                x.OnlineDeviceCount,
                x.DegradedDeviceCount,
                x.OfflineDeviceCount,
                x.AlertCount, 
                x.HealthScore,
                x.IsActive,
                
                x.LastSeenAt
            ))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<BranchListDto>(
            items,
            totalCount,
            page,
            pageSize
        );

        return ServiceResult<PagedResult<BranchListDto>>.Success(result);
    }
}
