using AuthServer.Domain.Roles;
using AuthServer.Domain.UserRoles;
using AuthServer.Domain.Users;
using FluentValidation;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using SharedLibrary.Abstractions.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace AuthServer.Application.Roles;
public sealed record AssignRoleCommand(Guid UserId,Guid RoleId):IRequest<ServiceResult>;

public sealed class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>

{
    public AssignRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.RoleId)
            .NotEmpty();
    }
}
public sealed class AssignRoleCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserRoleRepository userRoleRepository,
    IUnitOfWork unitOfWork)
        : IRequestHandler<AssignRoleCommand, ServiceResult>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ServiceResult> Handle(
        AssignRoleCommand request,
        CancellationToken cancellationToken)
    {
        var userId = new IdentityId(request.UserId);
        var roleId = new IdentityId(request.RoleId);

        var user = await _userRepository
            .GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return ServiceResult.Failure(
                "UserNotFound",
                "User not found",
                HttpStatusCode.NotFound);
        }

        var role = await _roleRepository
            .GetByIdAsync(roleId, cancellationToken);

        if (role is null)
        {
            return ServiceResult.Failure(
                "RoleNotFound",
                "Role not found",
                HttpStatusCode.NotFound);
        }

        var exists = await _userRoleRepository
            .ExistsAsync(userId,roleId,cancellationToken);


        if (exists)
        {
            return ServiceResult.Success();
        }

        var userRole = new UserRole(
            userId,
            roleId);

        await _userRoleRepository
            .AddAsync(userRole, cancellationToken);

        try
        {
            await _unitOfWork
                .SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Success();
        }

        return ServiceResult.Success();
    }
}