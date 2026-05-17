using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Services
{
    public interface IAlertService
    {
        Task HandleDeviceOfflineAsync(
            Device device,
            CancellationToken cancellationToken);

        Task HandleDeviceRecoveredAsync(
            Device device,
            CancellationToken cancellationToken);
    }
}
