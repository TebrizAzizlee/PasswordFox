

namespace NetWorkPassServer.Domain.Branches;
public enum BranchStatus
{
    Unknown = 0,
    Online = 2,
    Offline = 3,
    
    Maintenance = 4,
    Degraded = 5,
}