using FluentValidation;
using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Devices;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;
using static NetWorkPassServer.Domain.Devices.Device;

namespace NetWorkPassServer.Application.Devices;
public sealed record DeviceUpdateCommand(Guid Id,
    Guid BranchId,
    string Name,
    string IpAddress,
    int Type,
    string? Description):IRequest<ServiceResult>;

public sealed class DeviceUpdateCommandValidator : AbstractValidator<DeviceUpdateCommand>
{
    public DeviceUpdateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Device tapılmadı");

        RuleFor(x => x.BranchId)
            .NotEmpty().WithMessage("Branch seçilməlidir");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Device adı boş ola bilməz")
            .MaximumLength(100).WithMessage("Device adı maksimum 100 simvol ola bilər");

        RuleFor(x => x.IpAddress)
            .NotEmpty().WithMessage("IP address boş ola bilməz")
            .Must(ip => System.Net.IPAddress.TryParse(ip, out _))
            .WithMessage("IP düzgün formatda deyil");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Device tipi düzgün deyil");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Açıqlama maksimum 500 simvol ola bilər");
    }
}
internal sealed class DeviceUpdateCommandHandler(
    IDeviceRepository deviceRepository,
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeviceUpdateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(
        DeviceUpdateCommand request,
        CancellationToken cancellationToken)
    {
        // 🔥 1. Device var?
        var device = await deviceRepository.FirstOrDefaultAsync(
            x => x.Id == request.Id,
            cancellationToken);

        if (device is null)
        {
            return ServiceResult.Failure(
                "Tapılmadı",
                "Device mövcud deyil",
                HttpStatusCode.NotFound);
        }

        // 🔥 2. Branch var?
        var branchExists = await branchRepository.AnyAsync(
            x => x.Id == request.BranchId,
            cancellationToken);

        if (!branchExists)
        {
            return ServiceResult.Failure(
                "Branch tapılmadı",
                "Bu branch mövcud deyil",
                HttpStatusCode.NotFound);
        }

        // 🔥 3. Duplicate IP (ən kritik hissə)
        var ip = request.IpAddress.Trim();

        var exists = await deviceRepository.AnyAsync(
            x => x.Id != request.Id &&
                 x.BranchId == request.BranchId &&
                 x.Ip_Address.Value == ip,
            cancellationToken);

        if (exists)
        {
            return ServiceResult.Failure(
                "IP artıq mövcuddur",
                "Bu IP artıq istifadə olunur",
                HttpStatusCode.BadRequest);
        }

        // 🔥 4. ValueObject yarat
        var name = new DeviceName(request.Name);
        var ipAddress = new IpAddress(ip);
        var type = (DeviceType)request.Type;

        // 🔥 5. Update et
        device.Update(name, ipAddress, type, request.Description);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult.Success();
    }
}