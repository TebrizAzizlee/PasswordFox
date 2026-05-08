using AuthServer.Application.Dtos;
using AuthServer.Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using SharedLibrary.Abstractions.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace AuthServer.Application.Users;
public sealed record GetUsersQuery(
    int Page = 1,
    int Size = 10,
    string? Search = null,
    bool? IsActive = null)
    : IRequest<ServiceResult<PagedResult<UserListDto>>>;


public sealed class GetUsersQueryHandler(
    IUserRepository userRepository)
    : IRequestHandler<
        GetUsersQuery,
        ServiceResult<PagedResult<UserListDto>>>
{
    public async Task<ServiceResult<PagedResult<UserListDto>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Page <= 0)
            request = request with { Page = 1 };

        if (request.Size <= 0 || request.Size > 100)
            request = request with { Size = 10 };

        var query = userRepository
            .Where(x => !x.IsDeleted);

        // 🔥 SEARCH
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                x.UserName.Value.Contains(search) ||
                x.Email.Value.Contains(search));
        }

        // 🔥 STATUS FILTER
        if (request.IsActive.HasValue)
        {
            query = query.Where(x =>
                x.IsActive == request.IsActive.Value);
        }

        var totalCount = await query
            .CountAsync(cancellationToken);

        var users = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.Page - 1) * request.Size)
            .Take(request.Size)
            .Select(x => new UserListDto(
                x.Id.Value,
                x.FirstName.Value + " " + x.LastName.Value,
                x.UserName.Value,
                x.Email.Value,
                x.IsActive,
                x.UserRoles
                    .Select(r => r.Role.Name.Value)
                    .ToList()))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = new PagedResult<UserListDto>(users,totalCount,request.Page,request.Size);
       

        return ServiceResult<PagedResult<UserListDto>>
            .Success(result);
    }
}