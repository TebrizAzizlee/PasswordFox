using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Dtos
{
    public sealed record LiveMetricDto(
     Guid DeviceId,

     double? CpuUsage,

     double? MemoryUsage,

     double? Temperature,

     long? PingLatency,

     DateTime Timestamp
 );
}
