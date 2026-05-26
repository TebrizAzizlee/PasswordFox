using FluentValidation;
using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Devices;
using NetWorkPassServer.Domain.Shared;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace NetWorkPassServer.Application.Devices;
public sealed record DeviceUpdateCommand(
    Guid Id,
    string Name,
    string IpAddress,
    DeviceType Type,
    string Vendor,
    DeviceRole Role,
    string Model,
    bool IsCritical,
    string? Description):IRequest<ServiceResult>;

public sealed class DeviceUpdateCommandValidator : AbstractValidator<DeviceUpdateCommand>
{
    public DeviceUpdateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Device tapılmadı");


        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Device adı boş ola bilməz")
            .MaximumLength(100).WithMessage("Device adı maksimum 100 simvol ola bilər");

        RuleFor(x => x.IpAddress)
            .NotEmpty().WithMessage("IP address boş ola bilməz")
            .Must(ip => System.Net.IPAddress.TryParse(ip, out _))
            .WithMessage("IP düzgün formatda deyil");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Device tipi düzgün deyil");
        RuleFor(x => x.Vendor).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Model).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Role).IsInEnum();


        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Açıqlama maksimum 500 simvol ola bilər");
    }
}
internal sealed class DeviceUpdateCommandHandler(
    IDeviceRepository deviceRepository,
   
    IUnitOfWork unitOfWork
) : IRequestHandler<DeviceUpdateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(
        DeviceUpdateCommand request,
        CancellationToken cancellationToken)
    {
        // 🔥 1. Device var?
        var device = await deviceRepository.FirstOrDefaultAsync(
            x => x.Id == request.Id && !x.IsDeleted,
            cancellationToken);

        if (device is null)
        {
            return ServiceResult.Failure(
                "Tapılmadı",
                "Device mövcud deyil",
                HttpStatusCode.NotFound);
        }
        var ip =
           request.IpAddress.Trim();
        // 🔥 2. Branch var?
        var duplicateIpExists =
            await deviceRepository.AnyAsync(
                x =>
                    x.Id != request.Id &&
                    !x.IsDeleted &&
                    x.BranchId == device.BranchId &&
                    x.IpAddress.Value == ip,
                cancellationToken);

        if (duplicateIpExists)
        {
            return ServiceResult.Failure(
                "IP artıq mövcuddur",
                "Bu IP artıq branch daxilində istifadə olunur",
                HttpStatusCode.BadRequest);
        }

        // 🔥 4. ValueObject yarat
        var name = new DeviceName(request.Name);
        var ipAddress = new IpAddress(ip);
       
        // 🔥 5. Update et
        device.Update(name, ipAddress, request.Type,request.Vendor,request.Role, request.Model,request.IsCritical ,request.Description);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult.Success();
    }
}