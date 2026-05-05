using FluentValidation;
using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Devices;
using SharedLibrary;
using SharedLibrary.Abstractions.Entity;
using SharedLibrary.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;
using static NetWorkPassServer.Domain.Devices.Device;

namespace NetWorkPassServer.Application.Devices;
public sealed record DeviceCreateCommand(Guid BranchId,
    string Name,
    string IpAddress,
    int Type,
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
            .IsInEnum().WithMessage("Device tipi düzgün deyil");

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
        // 🔥 1. Branch var? (çox adam bunu unudur)
        var branchExists = await branchRepository.AnyAsync(
            x => x.Id == request.BranchId,
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
                 x.Ip_Address.Value == ip,
            cancellationToken);

        if (exists)
        {
            return ServiceResult<Guid>.Failure(
                "IP artıq mövcuddur",
                "Bu IP bu branch-də istifadə olunur",
                HttpStatusCode.BadRequest);
        }

        // 🔥 3. ValueObject yarat
        var name = new DeviceName(request.Name);
        var ipAddress = new IpAddress(request.IpAddress);

        var type = (DeviceType)request.Type;

        // 🔥 4. Entity yarat
        var device = new Device(
            new IdentityId(request.BranchId),
            name,
            ipAddress,
            type,
            request.Description
        );

        await deviceRepository.AddAsync(device, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<Guid>.Success(device.Id);
    }
}
