using AuthServer.Application.Services;
using SharedLibrary.Constants;
using SharedLibrary.Service;



namespace AuthServer.Infrastructure.Services;
public class FakeCurrentUserService : ICurrentUserService
{
    public Guid UserId => SystemUser.Id;

    public string? FullName => throw new NotImplementedException();
}
