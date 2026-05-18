using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Dtos
{
    public sealed record LiveMetricDto(
     Guid DeviceId,
      DateTime Timestamp,
     double? CpuUsage,

     double? MemoryUsage,
     double? DiskUsage ,

    double? Temperature,

     long? PingLatency

    
 );
}
