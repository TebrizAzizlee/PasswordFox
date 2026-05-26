using FluentValidation;
using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Branches.ValueObjects;
using SharedLibrary;

using System.Net;

using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record BranchUpdateCommand(
    Guid Id,
    string Code,
    string BranchName,
    string City,
    string District,
    string FullAddress,
    string PhoneNumber1,
    string? PhoneNumber2,
    string Email,
    string WanIp,
    string Subnet,
    string Gateway,
    string DnsServer,
    BranchType Type,
    string ? Description
    ) : IRequest<ServiceResult>;
public sealed class BranchUpdateCommandValidator : AbstractValidator<BranchUpdateCommand>
{
    public BranchUpdateCommandValidator()

    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MinimumLength(2).MaximumLength(20);
        RuleFor(i => i.BranchName).NotEmpty().WithMessage("Düzgün şöbə adı daxil edin");
        RuleFor(i => i.City).NotEmpty().WithMessage("Düzgün Şəhər adı daxil edin");
        RuleFor(i => i.FullAddress).NotEmpty().WithMessage("Düzgün Tam Adres daxil edin");
        RuleFor(i => i.PhoneNumber1).NotEmpty().Matches(@"^\+?\d{7,15}$").WithMessage("Düzgün Telefon nömrəsi daxil edin");
        RuleFor(i => i.Email).NotEmpty().EmailAddress().WithMessage("Düzgün e-poçt ünvanı daxil edin");
        RuleFor(x => x.WanIp).NotEmpty();
        RuleFor(x => x.Subnet).NotEmpty();
        RuleFor(x => x.Gateway).NotEmpty();
        RuleFor(x => x.DnsServer).NotEmpty();


    }
}
internal sealed class BranchUpdateCommandHandler(IBranchRepository branchRepository, IUnitOfWork unitOfWork) : IRequestHandler<BranchUpdateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(BranchUpdateCommand request, CancellationToken cancellationToken)
    {
        
        var branch = await branchRepository.FirstOrDefaultAsync(i => i.Id==request.Id && !i.IsDeleted, cancellationToken);
        if (branch is null)
        {
            return ServiceResult.Failure("Tapılmadı", "Şöbə tapılmadı", HttpStatusCode.NotFound);
        }
        
        var exists=await branchRepository.AnyAsync(x=>x.Id!=request.Id && !x.IsDeleted && x.Code==request.Code && x.Name.Value==request.BranchName,cancellationToken);
      if(exists)
        {
            return ServiceResult.Failure(
           "Şöbə artıq mövcuddur",
           "Bu kodda və ya adda başqa şöbə mövcuddur",
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
        branch.ChangeCode(request.Code);
        branch.Update(name,request.Type, address,contactInfo,networkInfo,request.Description);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult.Success();
    }
}
