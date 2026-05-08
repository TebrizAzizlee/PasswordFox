using AuthServer.Application.Dtos;
using AuthServer.Domain.Roles;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using SharedLibrary.Abstractions.Pagination;
using TS.MediatR;

namespace AuthServer.Application.Roles;
public sealed record GetRolesQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null)
    : IRequest<ServiceResult<PagedResult<RoleListDto>>>;

public sealed class GetRolesQueryHandler(
    IRoleRepository roleRepository)
    : IRequestHandler<
        GetRolesQuery,
        ServiceResult<PagedResult<RoleListDto>>>
{
    public async Task<ServiceResult<PagedResult<RoleListDto>>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Page <= 0)
            request = request with { Page = 1 };

        if (request.PageSize <= 0 || request.PageSize > 100)
            request = request with { PageSize = 10 };

        var query = roleRepository
            .Where(x => !x.IsDeleted);

        // 🔥 SEARCH

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                x.Name.Value.Contains(search));
        }

        var totalCount = await query
            .CountAsync(cancellationToken);

        var roles = await query
            .OrderBy(x => x.Name.Value)
            .Select(x => new RoleListDto(
                x.Id.Value,
                x.Name.Value,
                x.Description,
                x.IsActive,
                x.UserRoles.Count()))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var pagedRoles = roles
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var result = new PagedResult<RoleListDto>(
            pagedRoles,
            totalCount,
            request.Page,
            request.PageSize);

        return ServiceResult<PagedResult<RoleListDto>>
            .Success(result);
    }
}