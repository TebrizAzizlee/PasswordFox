using AuthServer.Domain.Roles;
using AuthServer.Domain.Roles.ValueObjects;
using FluentValidation;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace AuthServer.Application.Roles;
public sealed record CreateRoleCommand(string Name, string? Description) : IRequest<ServiceResult<Guid>>;

public sealed class CreateRoleCommandValidator
    : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(64);

        RuleFor(x => x.Description)
            .MaximumLength(256);
    }
}
public sealed class CreateRoleCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork)
        : IRequestHandler<CreateRoleCommand, ServiceResult<Guid>>
{
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ServiceResult<Guid>> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        RoleName roleName;

        try
        {
            roleName = new RoleName(request.Name);
        }
        catch (Exception ex)
        {
            return ServiceResult<Guid>.Failure(
                "InvalidRoleName",
                ex.Message,
                HttpStatusCode.BadRequest);
        }

        var exists = await _roleRepository
            .ExistsAsync(roleName, cancellationToken);

        if (exists)
        {
            return ServiceResult<Guid>.Failure(
                "RoleAlreadyExists",
                "Role already exists",
                HttpStatusCode.Conflict);
        }

        var role = new Role(
            roleName,
            request.Description);

        await _roleRepository
            .AddAsync(role, cancellationToken);

        try
        {
            await _unitOfWork
                .SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return ServiceResult<Guid>.Failure(
                "RoleAlreadyExists",
                "Role already exists",
                HttpStatusCode.Conflict);
        }

        return ServiceResult<Guid>.Success(role.Id.Value);
    }
}
