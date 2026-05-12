using AuthServer.Application.Dtos;
using AuthServer.Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using SharedLibrary.Abstractions.Entity;
using System.Net;
using TS.MediatR;

namespace AuthServer.Application.Users;
public sealed record GetUserByIdQuery(
    Guid UserId)
    : IRequest<ServiceResult<UserDetailDto>>;

public sealed class GetUserByIdQueryHandler(
    IUserRepository userRepository)
    : IRequestHandler<
        GetUserByIdQuery,
        ServiceResult<UserDetailDto>>
{
    public async Task<ServiceResult<UserDetailDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var userId =
            new IdentityId(request.UserId);

        var user = await userRepository
            .Where(x =>
                x.Id == userId &&
                !x.IsDeleted)
            .Select(x =>
                new UserDetailDto(
                    x.Id.Value,
                    x.FirstName.Value,
                    x.LastName.Value,
                    x.FullName.Value,
                    x.UserName.Value,
                    x.Email.Value,
                    x.UserRoles
                        .Select(r =>
                            r.Role.Name.Value)
                        .Distinct()
                        .ToList(),
                    x.CreatedAt))
            .FirstOrDefaultAsync(
                cancellationToken);

        if (user is null)
        {
            return ServiceResult<UserDetailDto>
                .Failure(
                    "UserNotFound",
                    "User not found",
                    HttpStatusCode.NotFound);
        }

        return ServiceResult<UserDetailDto>
            .Success(user);
    }
}