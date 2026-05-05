using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Dtos;
using NetWorkPassServer.Domain.Devices;
using SharedLibrary;
using SharedLibrary.Abstractions.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace NetWorkPassServer.Application.Devices;
public sealed record DeviceGetAllQuery(Guid? BranchId,
    string? Search,          // name və ya ip üçün
    int? Type,
    int Page = 1,
    int PageSize = 10):IRequest<ServiceResult<PagedResult<DeviceDto>>>;

internal sealed class DeviceGetAllQueryHandler(
    IDeviceRepository deviceRepository
) : IRequestHandler<DeviceGetAllQuery, ServiceResult<PagedResult<DeviceDto>>>
{
    public async Task<ServiceResult<PagedResult<DeviceDto>>> Handle(
        DeviceGetAllQuery request,
        CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 10 : request.PageSize;

        var query = deviceRepository.Where(x => true); // IQueryable əldə et

        // 🔥 1. Branch filter
        if (request.BranchId.HasValue)
        {
            query = query.Where(x => x.BranchId == request.BranchId.Value);
        }

        // 🔥 2. Type filter
        if (request.Type.HasValue)
        {
            query = query.Where(x => (int)x.Type == request.Type.Value);
        }

        // 🔥 3. Search (Name + IP)
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
               EF.Functions.Like( x.Name.Value, $"%{search}%") ||
               EF.Functions.Like(x.Ip_Address.Value, $"%{search}%"));
        }

        // 🔥 4. Count
        var totalCount = await query.CountAsync(cancellationToken);

        // 🔥 5. Pagination + projection
        var items = await query
            .OrderBy(x => x.Name.Value)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new DeviceDto(
                x.Id,
                x.BranchId,
                x.Name.Value,
                x.Ip_Address.Value,
                (int)x.Type,
                x.Description,
                x.IsActive
            ))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<DeviceDto>(
            items,
            totalCount,
            page,
            pageSize
        );

        return ServiceResult<PagedResult<DeviceDto>>.Success(result);
    }
}