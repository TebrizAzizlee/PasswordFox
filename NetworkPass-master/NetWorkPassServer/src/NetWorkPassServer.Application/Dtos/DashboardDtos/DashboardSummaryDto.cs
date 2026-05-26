using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Dtos.DashboardDtos;
public sealed record DashboardSummaryDto
(int TotalBranches,

    int OnlineBranches,

    int OfflineBranches,

    int DegradedBranches,

    int TotalDevices,

    int OnlineDevices,

    int OfflineDevices,

    int DegradedDevices,

    int ActiveAlerts,

    int CriticalAlerts,

    int WarningAlerts);
