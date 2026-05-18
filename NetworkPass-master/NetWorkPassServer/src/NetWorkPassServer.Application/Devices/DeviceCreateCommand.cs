using FluentValidation;
using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Devices;
using SharedLibrary;

using SharedLibrary.Consts;
using System.Net;
using TS.MediatR;



namespace NetWorkPassServer.Application.Devices;
public sealed record DeviceCreateCommand(Guid BranchId,
    string Name,
    string IpAddress,
    DeviceType Type,
    string? Description):IRequest<ServiceResult<Guid>>;

public sealed class DeviceCreateCommandValidator : AbstractValidator<DeviceCreateCommand>
{
    public DeviceCreateCommandValidator()
    {
        RuleFor(x => x.BranchId)
            .NotEmpty().WithMessage("Branch seçilməlidir");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(EntityConsts.MaxNameLength).WithMessage("Device adı maksimum 100 simvol ola bilər");

        RuleFor(x => x.IpAddress)
            .NotEmpty().WithMessage("IP address boş ola bilməz")
            .Must(ip => System.Net.IPAddress.TryParse(ip, out _))
            .WithMessage("IP address düzgün formatda deyil");

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Description)
            .MaximumLength(EntityConsts.MaxDesrictionLength).WithMessage("Açıqlama maksimum 500 simvol ola bilər"); ;
    }
}
internal sealed class DeviceCreateCommandHandler(
    IDeviceRepository deviceRepository,
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeviceCreateCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(
        DeviceCreateCommand request,
        CancellationToken cancellationToken)
    {
        var branchId = request.BranchId;

        // 🔥 1. Branch var? (çox adam bunu unudur)
        var branchExists = await branchRepository.AnyAsync(
            x => x.Id == branchId,
            cancellationToken);

        if (!branchExists)
        {
            return ServiceResult<Guid>.Failure(
                "Branch tapılmadı",
                "Bu branch mövcud deyil",
                HttpStatusCode.NotFound);
        }

        // 🔥 2. Duplicate IP check
        var ip = request.IpAddress.Trim();

        var exists = await deviceRepository.AnyAsync(
            x => x.BranchId == request.BranchId &&
                 x.IpAddress.Value == ip,
            cancellationToken);

        if (exists)
        {
            return ServiceResult<Guid>.Failure(
                "IP artıq mövcuddur",
                "Bu IP bu branch-də istifadə olunur",
                HttpStatusCode.BadRequest);
        }

        DeviceName deviceName;
        IpAddress ipAddress;
      
        try
        {
            // 🔥 3. ValueObject yarat
            deviceName = new DeviceName(request.Name);
            ipAddress = new IpAddress(request.IpAddress);
        }
        catch (Exception ex)
        {

            return ServiceResult<Guid>.Failure("ValidationError",ex.Message,HttpStatusCode.BadRequest);
        }
       

        // 🔥 4. Entity yarat
        var device = new Device(
            request.BranchId,
            deviceName,
            ipAddress,
            request.Type,
            request.Description
            
        );

        await deviceRepository.AddAsync(device, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<Guid>.Success(device.Id);
    }
}
