

namespace AuthServer.Application.Services;

    public interface IUserContext
    {
        Guid GetUserId();
    IReadOnlyCollection<string> GetPermissions();
}

