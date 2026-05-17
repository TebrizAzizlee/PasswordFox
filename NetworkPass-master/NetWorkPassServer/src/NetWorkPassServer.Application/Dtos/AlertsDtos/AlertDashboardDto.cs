using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Dtos.AlertsDtos
{
    public sealed record AlertDashboardDto(
    int TotalActiveAlerts,

    int CriticalAlerts,

    int WarningAlerts,

    int InfoAlerts,

    List<AlertListDto> LatestAlerts
);
}
