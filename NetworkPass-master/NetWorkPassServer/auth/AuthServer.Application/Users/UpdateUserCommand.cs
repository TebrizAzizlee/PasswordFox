using AuthServer.Domain.Users;
using AuthServer.Domain.Users.ValueObjects;
using FluentValidation;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using SharedLibrary.Abstractions.Entity;
using System.Net;
using TS.MediatR;

namespace AuthServer.Application.Users;
public sealed record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    bool IsActive)
    : IRequest<ServiceResult>;


public sealed class UpdateUserCommandValidator
    : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .MinimumLength(2)
            .MaximumLength(64);

        RuleFor(x => x.LastName)
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .MinimumLength(2)
            .MaximumLength(64);

        RuleFor(x => x.Email)
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .EmailAddress()
            .MaximumLength(256);
    }
}


public sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var userId = new IdentityId(request.UserId);

        var user = await userRepository
            .GetByIdAsync(
                userId,
                cancellationToken);

        if (user is null)
        {
            return ServiceResult.Failure(
                "UserNotFound",
                "User not found",
                HttpStatusCode.NotFound);
        }

        Email email;
        FirstName firstName;
        LastName lastName;

        try
        {
            email = new Email(request.Email.Trim());

            firstName = new FirstName(
                request.FirstName.Trim());

            lastName = new LastName(
                request.LastName.Trim());
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure(
                "ValidationError",
                ex.Message,
                HttpStatusCode.BadRequest);
        }

        var emailExists = await userRepository
            .ExistsByEmailAsync(
                email,
                cancellationToken);

        if (emailExists &&
            user.Email != email)
        {
            return ServiceResult.Failure(
                "EmailAlreadyExists",
                "Email already exists",
                HttpStatusCode.Conflict);
        }

        


        user.SetEmail(email);

        user.SetStatus(request.IsActive);

        try
        {
            await unitOfWork
                .SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Failure(
                "UpdateFailed",
                "User update failed",
                HttpStatusCode.Conflict);
        }

        return ServiceResult.Success();
    }
}