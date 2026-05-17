using NetWorkPassServer.Domain.Branches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Dtos.BranchDtos
{
    public sealed record BranchRealtimeDto(
      Guid Id,

      string Name,

      BranchStatus Status,

      int TotalDevices,

      int OnlineDevices,

      int OfflineDevices,

      int ActiveAlerts,

      DateTime? LastSeenAt
  );
}
