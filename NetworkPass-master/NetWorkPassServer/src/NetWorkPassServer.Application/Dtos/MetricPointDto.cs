using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Dtos
{
    public sealed record MetricPointDto(
     DateTime Timestamp,

     double? CpuUsage,

     double? MemoryUsage,

     double? Temperature,

     long? PingLatency
 );
}
