using NetWorkPassServer.Application.Dtos.BranchDtos;
using NetWorkPassServer.Application.Dtos.DeviesDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Notifications;
public interface IMonitoringRealtimeNotifier
{
    Task DeviceStatusChangedAsync(
       DeviceRealtimeDto dto,
       CancellationToken cancellationToken);

   
}
