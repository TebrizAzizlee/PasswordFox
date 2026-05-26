using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Services;
public interface  IBranchStatsService
{
    Task RecalculateAsync(
        Guid branchId,
        CancellationToken cancellationToken);
}
