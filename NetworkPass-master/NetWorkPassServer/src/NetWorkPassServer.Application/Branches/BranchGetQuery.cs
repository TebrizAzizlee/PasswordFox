using Microsoft.EntityFrameworkCore;
using NetWorkPassServer.Application.Dtos;
using NetWorkPassServer.Domain.Branches;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record BranchGetQuery(Guid Id) : IRequest<ServiceResult<BranchDto>>;

internal sealed class BranchGetQueryHandler(IBranchRepository branchRepository) : IRequestHandler<BranchGetQuery, ServiceResult<BranchDto>>
{
    public async Task<ServiceResult<BranchDto>> Handle(BranchGetQuery request, CancellationToken cancellationToken)
    {

        var branch = await branchRepository
         .Where(x => x.Id == request.Id)
         .Select(x => new BranchDto(
             x.Id,
             x.Name.Value,
             x.Address.City,
             x.Address.FullAddress,
             x.Address.PhoneNumber1,
             x.Address.Email,
             x.IsActive
         ))
         .SingleOrDefaultAsync(cancellationToken);

        if (branch is null)
        {
            return ServiceResult<BranchDto>.Failure(
                "Tapılmadı",
                "Şöbə mövcud deyil",
                HttpStatusCode.NotFound);
        }

        return ServiceResult<BranchDto>.Success(branch);
    }
}

