

using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Application.Dtos.DeviesDtos;
using SharedLibrary;
using SharedLibrary.Abstractions.Pagination;
using TS.MediatR;

namespace NetWorkPassServer.Application.Devices;
public sealed record DeviceMonitoringListQuery(Guid? BranchId, string? Search, int Page = 1, int PageSize = 10)
    : IRequest<ServiceResult<PagedResult<DeviceMonitoringItemDto>>>;

internal sealed class DeviceMonitoringListQueryHandler(IPasswordDbContext context) : IRequestHandler<DeviceMonitoringListQuery, ServiceResult<PagedResult<DeviceMonitoringItemDto>>> 
{ public async Task<ServiceResult<PagedResult<DeviceMonitoringItemDto>>> Handle(DeviceMonitoringListQuery request, CancellationToken cancellationToken) 
    { var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 10 : request.PageSize; 
        var query = context.Devices.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive); 
        // Branch Filter
         if (request.BranchId.HasValue) 
        { query = query.Where( x => x.BranchId == request.BranchId.Value); }
         // Search
          if (!string.IsNullOrWhiteSpace( request.Search)) 
        { var search = request.Search.Trim(); 
          query = query.Where( x => EF.Functions.Like( x.Name.Value, $"%{search}%") 
          || EF.Functions.Like( x.IpAddress.Value, $"%{search}%")); } 
        var totalCount = await query.CountAsync( cancellationToken);
        var items = await query .OrderBy(x => x.Name.Value)
            .Skip( (page - 1) * pageSize) 
            .Take(pageSize) 
            .Select(x => new DeviceMonitoringItemDto(
                x.Id,
                x.Name.Value,
                x.IpAddress.Value,
                x.Type, x.Status,
                x.CpuUsage,
                x.MemoryUsage,
                x.PingLatency,
                x.LastSeenAt )) 
            .ToListAsync( cancellationToken);
        var result = new PagedResult< DeviceMonitoringItemDto>( items, totalCount, page, pageSize);
        return ServiceResult< PagedResult< DeviceMonitoringItemDto>>.Success(result); } }