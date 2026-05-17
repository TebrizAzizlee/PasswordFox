using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Dtos;
public sealed class DashboardSummaryDto
{
    public int TotalBranches { get; set; }

    public int OnlineBranches { get; set; }

    public int OfflineBranches { get; set; }

    public int WarningBranches { get; set; }

    public int TotalDevices { get; set; }

    public int OnlineDevices { get; set; }

    public int OfflineDevices { get; set; }

    public int ActiveAlerts { get; set; }

    public int CriticalAlerts { get; set; }
}
