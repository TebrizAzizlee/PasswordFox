using GenericRepository;
using NetWorkPassServer.Domain.Devices;
using NetWorkPassServer.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Infrastructure.Repositories;
internal sealed class DeviceRepository(PasswordDbContext context):Repository<Device, PasswordDbContext>(context),IDeviceRepository
{
}
