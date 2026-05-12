

namespace AuthServer.Application.Dtos;
public sealed record UserDetailDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string UserName,
    string Email,
    List<string> Roles,
    DateTimeOffset CreatedDate);