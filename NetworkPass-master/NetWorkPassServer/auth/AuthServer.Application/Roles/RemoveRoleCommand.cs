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
public sealed record RemoveRoleCommand(
    Guid UserId,
    Guid RoleId)
    : IRequest<ServiceResult>;
public sealed class RemoveRoleCommandValidator
    : AbstractValidator<RemoveRoleCommand>
{
    public RemoveRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.RoleId)
            .NotEmpty();
    }
}
public sealed class RemoveRoleCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserRoleRepository userRoleRepository,
    IUnitOfWork unitOfWork)
        : IRequestHandler<RemoveRoleCommand, ServiceResult>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ServiceResult> Handle(
        RemoveRoleCommand request,
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

        var userRole = await _userRoleRepository
            .GetAsync(
                userId,
                roleId,
                cancellationToken);

        // idempotent remove
        if (userRole is null)
        {
            return ServiceResult.Success();
        }

        // 🔥 CRITICAL SECURITY RULE
        // prevent removing last admin

        if ((string)role.Name == "Admin")
        {
            var adminCount = await _userRoleRepository
                .Where(x => x.RoleId == roleId)
                .CountAsync(cancellationToken);

            if (adminCount <= 1)
            {
                return ServiceResult.Failure(
                    "LastAdmin",
                    "Last admin role cannot be removed",
                    HttpStatusCode.BadRequest);
            }
        }

        _userRoleRepository.Remove(userRole);

        await _unitOfWork
            .SaveChangesAsync(cancellationToken);

        return ServiceResult.Success();
    }
}