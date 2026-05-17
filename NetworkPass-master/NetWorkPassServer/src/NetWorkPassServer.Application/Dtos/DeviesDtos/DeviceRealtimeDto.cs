using NetWorkPassServer.Domain.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Dtos.DeviesDtos
{
    public sealed record DeviceRealtimeDto(
     Guid Id,

     Guid BranchId,

     string Name,

     DeviceStatus Status,

     long? PingLatency,

     double? CpuUsage,

     double? MemoryUsage,

     double? Temperature,

     DateTime? LastSeenAt
 );
}
