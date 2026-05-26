using GenericRepository;
using NetWorkPassServer.Domain.Alerts;
using NetWorkPassServer.Infrastructure.Context;


namespace NetWorkPassServer.Infrastructure.Repositories
{
    internal sealed class AlertRepository(
        PasswordDbContext context)
            : Repository<Alert, PasswordDbContext>(context),
        IAlertRepository
    {
    }
}
