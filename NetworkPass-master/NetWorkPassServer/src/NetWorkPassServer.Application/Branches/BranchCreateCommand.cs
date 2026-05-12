using FluentValidation;
using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Branches.ValueObjects;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record BranchCreateCommand(string Name,
    string City,
    string District,
    string FullAddress,
    string PhoneNumber1,
    string? PhoneNumber2,
    string Email) : IRequest<ServiceResult<Guid>>;

public sealed class BranchCreateCommmandValidator : AbstractValidator<BranchCreateCommand>
{
    public BranchCreateCommmandValidator()
    {
        RuleFor(i => i.Name).NotEmpty().WithMessage("Düzgün şöbə adı daxil edin");
        RuleFor(i => i.City).NotEmpty().WithMessage("Düzgün Şəhər adı daxil edin");
        RuleFor(i => i.FullAddress).NotEmpty().WithMessage("Düzgün Tam Adres daxil edin");
        RuleFor(i => i.PhoneNumber1).NotEmpty().Matches(@"^\+?\d{7,15}$").WithMessage("Düzgün Telefon nömrəsi daxil edin");
        RuleFor(i => i.Email).NotEmpty().EmailAddress().WithMessage("Düzgün e-poçt ünvanı daxil edin");
    }
}
internal sealed class BranchCreateCommandHandler(IBranchRepository branchRepository, IUnitOfWork unitOfWork) : IRequestHandler<BranchCreateCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(BranchCreateCommand request, CancellationToken cancellationToken)
    {
        
        var exists = await branchRepository.AnyAsync(p => p.Name.Value == request.Name , cancellationToken);
        if (exists)
        {
            return ServiceResult<Guid>.Failure(
                "Şöbə artıq mövcuddur",
                "Bu adda şöbə sistemdə var",
                System.Net.HttpStatusCode.BadRequest);
        }
        Name name = new(request.Name);
        var address = new Address
        (request.City,
    request.District,
    request.FullAddress,
    request.PhoneNumber1,
    request.PhoneNumber2,
    request.Email);
          
   
        Branch branch = new(name, address);
       await branchRepository.AddAsync(branch,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<Guid>.Success(branch.Id);

    }
}