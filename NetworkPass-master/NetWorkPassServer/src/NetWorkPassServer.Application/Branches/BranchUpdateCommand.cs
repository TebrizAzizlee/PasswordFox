using FluentValidation;
using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Branches.ValueObjects;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record BranchUpdateCommand(Guid Id, string Name, string City,
    string District,
    string FullAddress,
    string PhoneNumber1,
    string? PhoneNumber2,
    string Email) : IRequest<ServiceResult>;
public sealed class BranchUpdateCommandValidator : AbstractValidator<BranchUpdateCommand>
{
    public BranchUpdateCommandValidator()

    {
        RuleFor(i => i.Name).NotEmpty().WithMessage("Düzgün şöbə adı daxil edin");
        RuleFor(i => i.City).NotEmpty().WithMessage("Düzgün Şəhər adı daxil edin");
        RuleFor(i => i.FullAddress).NotEmpty().WithMessage("Düzgün Tam Adres daxil edin");
        RuleFor(i => i.PhoneNumber1).NotEmpty().Matches(@"^\+?\d{7,15}$").WithMessage("Düzgün Telefon nömrəsi daxil edin");
        RuleFor(i => i.Email).NotEmpty().EmailAddress().WithMessage("Düzgün e-poçt ünvanı daxil edin");


    }
}
internal sealed class BranchUpdateCommandHandler(IBranchRepository branchRepository, IUnitOfWork unitOfWork) : IRequestHandler<BranchUpdateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(BranchUpdateCommand request, CancellationToken cancellationToken)
    {
        
        var branch = await branchRepository.FirstOrDefaultAsync(i => i.Id==request.Id , cancellationToken);
        if (branch is null)
        {
            return ServiceResult.Failure("Tapılmadı", "Şöbə tapılmadı", HttpStatusCode.NotFound);
        }
        
        var exists=await branchRepository.AnyAsync(x=>x.Id!=request.Id && !x.IsDeleted && x.Name.Value==request.Name,cancellationToken);
      if(exists)
        {
            return ServiceResult.Failure(
           "Şöbə artıq mövcuddur",
           "Bu adda başqa şöbə var",
           HttpStatusCode.BadRequest);
        }
        Name name = new(request.Name);
        Address address = new (
           
            request.City,
            request.District,
            request.FullAddress,
            request.PhoneNumber1,
            request.PhoneNumber2,
            request.Email);
        branch.Update(name, address);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult.Success();
    }
}
