using AuthServer.Domain.Roles;
using AuthServer.Domain.Roles.ValueObjects;
using AuthServer.Domain.UserRoles;
using AuthServer.Domain.Users;
using AuthServer.Domain.Users.ValueObjects;
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

namespace AuthServer.Application.Users;
public sealed record CreateUserCommand(string FirstName,string LastName, string UserName, string Email, string Password) : IRequest<ServiceResult<Guid>>;

public sealed class CreateUserCommandValidator
    : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .MinimumLength(2)
            .MaximumLength(64);

        RuleFor(x => x.LastName)
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .MinimumLength(2)
            .MaximumLength(64);
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(64);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .Must(x=>!string.IsNullOrWhiteSpace(x))
            .MinimumLength(8)
            .MaximumLength(128);
    }
}
public sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserRoleRepository userRoleRepository,
    IUnitOfWork unitOfWork)
        : IRequestHandler<CreateUserCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        FirstName firstName;
        LastName lastName;
        Email email;
        UserName userName;
        Password password;

        try
        {
            firstName = new FirstName(request.FirstName.Trim());
            lastName = new LastName(request.LastName.Trim());
            email = new Email(request.Email.Trim());
            userName = new UserName(request.UserName.Trim());
            password = new Password(request.Password);
        }
        catch (Exception ex)
        {

            return ServiceResult<Guid>.Failure(
              "ValidationError",
              ex.Message,
              HttpStatusCode.BadRequest);
        }

        var emailExists = await userRepository
            .ExistsByEmailAsync(email,cancellationToken);


        if (emailExists)
        {
            return ServiceResult<Guid>.Failure(
                "EmailAlreadyExists",
                "Email already exists",
                HttpStatusCode.Conflict);
        }

        var userNameExists = await userRepository
            .ExistsByUserNameAsync(userName, cancellationToken);

        if (userNameExists)
        {
            return ServiceResult<Guid>.Failure(
                "UserNameAlreadyExists",
                "Username already exists",
                HttpStatusCode.Conflict);
        }


        // 🔥 DEFAULT ROLE
        var defaultRole = await roleRepository
            .GetByNameAsync(
                SystemRoles.User,
                cancellationToken);

        if (defaultRole is null)
        {
            return ServiceResult<Guid>.Failure(
                "DefaultRoleMissing",
                "Default role not configured",
                HttpStatusCode.InternalServerError);
        }

        var user = User.Create(firstName,
            lastName, userName, email, password);
        await userRepository
          .AddAsync(user, cancellationToken);

        var userRole = new UserRole(
            user.Id,
            defaultRole.Id);

        await userRoleRepository
            .AddAsync(userRole, cancellationToken);

        try
        {
            await unitOfWork
                .SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return ServiceResult<Guid>.Failure(
                "UserAlreadyExists",
                "User already exists",
                HttpStatusCode.Conflict);
        }

        return ServiceResult<Guid>
            .Success(user.Id.Value);
    }
}

