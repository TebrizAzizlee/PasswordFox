using GenericRepository;
using NetWorkPassServer.Domain.Alerts;
using NetWorkPassServer.Infrastructure.Context;


namespace NetWorkPassServer.Infrastructure.Repositories
{
    internal sealed class AlertRepository
      : Repository<Alert, PasswordDbContext>,
        IAlertRepository
    {
        public AlertRepository(
            PasswordDbContext context)
            : base(context)
        {
        }
    }
}
