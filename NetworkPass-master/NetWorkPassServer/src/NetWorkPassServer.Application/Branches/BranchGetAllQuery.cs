using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Dtos;
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
    int Page = 1,
    int PageSize = 10
) : IRequest<ServiceResult<PagedResult<BranchDto>>>;


internal sealed class BranchGetAllQueryHandler(
IBranchRepository branchRepository
) : IRequestHandler<BranchGetAllQuery, ServiceResult<PagedResult<BranchDto>>>
{
   
    public async Task<ServiceResult<PagedResult<BranchDto>>> Handle(
        BranchGetAllQuery request,
        CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 10 : request.PageSize;
        var query = branchRepository.Where(x=>true);
            

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Name.Value) // default sorting
            .Skip((page-1)*pageSize)
            .Take(pageSize)
            .Select(x => new BranchDto(
                x.Id,
                x.Name.Value,
                x.Address.City,
                x.Address.FullAddress,
                x.Address.PhoneNumber1,
                x.Address.Email,
                x.IsActive
            ))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<BranchDto>(
            items,
            totalCount,
            request.Page,
            request.PageSize
        );

        return ServiceResult<PagedResult<BranchDto>>.Success(result);
    }
}
