using FluentValidation;
using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Branches.ValueObjects;
using SharedLibrary;
using TS.MediatR;

namespace NetWorkPassServer.Application.Branches;
public sealed record BranchCreateCommand(string Name,
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
    BranchType type,
    string? description,
    string Code,
    int healtScore
    ) : IRequest<ServiceResult<Guid>>;

public sealed class BranchCreateCommmandValidator : AbstractValidator<BranchCreateCommand>
{
    public BranchCreateCommmandValidator()
    {
        RuleFor(i => i.Code).NotEmpty().MinimumLength(2).MaximumLength(20);
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
        
        var exists = await branchRepository.AnyAsync(p =>  p.Code==request.Code||p.Name.Value == request.Name , cancellationToken);
        if (exists)
        {
            return ServiceResult<Guid>.Failure(
                "Şöbə artıq mövcuddur",
                "Bu adda şöbə sistemdə var",
                System.Net.HttpStatusCode.BadRequest);
        }
        BranchName name = new(request.Name);
        
        var address = new Address
        (request.City,
    request.District,
    request.FullAddress
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
        Branch branch = new(name,request.type, address,contactInfo,networkInfo,request.Code, request.description, request.healtScore);
       await branchRepository.AddAsync(branch,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<Guid>.Success(branch.Id);

    }
}