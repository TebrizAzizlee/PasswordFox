using GenericRepository;
using NetWorkPassServer.Domain.DeviceHeartbeats;
using NetWorkPassServer.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Infrastructure.Repositories
{
    internal sealed class DeviceHeartbeatRepository
    : Repository<DeviceHeartbeat, PasswordDbContext>,
      IDeviceHeartbeatRepository
    {
        public DeviceHeartbeatRepository(
            PasswordDbContext context)
            : base(context)
        {
        }
    }
}
