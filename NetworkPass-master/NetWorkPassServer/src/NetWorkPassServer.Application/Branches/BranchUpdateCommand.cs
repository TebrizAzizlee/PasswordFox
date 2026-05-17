using FluentValidation;
using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Branches.ValueObjects;
using SharedLibrary;

using System.Net;

using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record BranchUpdateCommand(Guid Id, string BranchName, string City,
    string District,
    string FullAddress,
    string PhoneNumber1,
    string? PhoneNumber2,
    string Email,
    string WanIp,
    string Subnet,
    string Gateway,
    string DnsServer
    ) : IRequest<ServiceResult>;
public sealed class BranchUpdateCommandValidator : AbstractValidator<BranchUpdateCommand>
{
    public BranchUpdateCommandValidator()

    {
        RuleFor(i => i.BranchName).NotEmpty().WithMessage("Düzgün şöbə adı daxil edin");
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
        
        var exists=await branchRepository.AnyAsync(x=>x.Id!=request.Id && !x.IsDeleted && x.Name==request.BranchName,cancellationToken);
      if(exists)
        {
            return ServiceResult.Failure(
           "Şöbə artıq mövcuddur",
           "Bu adda başqa şöbə var",
           HttpStatusCode.BadRequest);
        }
        BranchName name = new(request.BranchName);
        var address = new Address
       (city: request.City,
        district: request.District,
        fullAddress:  request.FullAddress
   );
        var contactInfo = new ContactInfo(
            request.PhoneNumber1,
            request.PhoneNumber2,
            request.Email
            );
        var networkInfo = new NetworkInfo(
            request.WanIp,
            request.Subnet,
            request.Gateway,
            request.DnsServer
            );
        branch.Update(name, address,contactInfo,networkInfo);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult.Success();
    }
}
