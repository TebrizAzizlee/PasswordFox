using Abp.Domain.Entities.Auditing;
using NetWorkPassServer.Domain.Alerts;
using NetWorkPassServer.Domain.Branches.ValueObjects;
using NetWorkPassServer.Domain.Devices;
using NetWorkPassServer.Domain.VpnTunnels;

namespace NetWorkPassServer.Domain.Branches;
public sealed class Branch : FullAuditedAggregateRoot<Guid>
{
    private Branch()
    {
    }

    public Branch(
        BranchName name,
        BranchType type,
        Address address,
        ContactInfo contactInfo,
        NetworkInfo networkInfo,
        string code, 
        string? description,
        int healtScore)

    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Branch code boş ola bilməz");
        }

        Code = code.Trim().ToUpperInvariant();
       Name=name;
        Type = type;
        Description = description?.Trim();
        HealthScore = healtScore;
        Address = address;
        ContactInfo = contactInfo;
        NetworkInfo = networkInfo;

        Devices = [];
        Alerts = [];
        VpnTunnels = [];

        Status = BranchStatus.Unknown;
        HealthScore=100;
        IsActive = true;
        IsMonitoringEnabled = true;
        IsInMaintenanceMode = false;
    }

    // BASIC

    public string Code { get; private set; } = default!;

    public BranchName Name { get; private set; } = default!;

    public BranchType Type { get; private set; }

    public string? Description { get; private set; }=default!;


    // VALUE OBJECTS
    public Address Address { get; private set; } = default!;

    public ContactInfo ContactInfo { get; private set; } = default!;

    public NetworkInfo NetworkInfo { get; private set; } = default!;


    // STATUS

    public BranchStatus Status { get; private set; }

    public bool IsActive { get; private set; } = default!;

    public bool IsMonitoringEnabled { get; private set; } = default!;

    public bool IsInMaintenanceMode { get; private set; } = default!;

    public DateTime? LastSeenAt { get; private set; } = default!;


    // STATS

    public int TotalDeviceCount { get; private set; } = default!;

    public int OnlineDeviceCount { get; private set; }=default!;
        
    public int OfflineDeviceCount { get; private set; } = default!;

    public int DegradedDeviceCount { get; private set; } = default!;

    public int AlertCount { get; private set; } = default!;

    public int HealthScore { get; private set; } = default!;
    // RELATIONS


    public ICollection<Device> Devices { get; private set; } = default!;

    public ICollection<Alert> Alerts { get; private set; } = default!;

    public ICollection<VpnTunnel> VpnTunnels { get; private set; } = default!;


    //public void SetName(BranchName name)
    //{
    //    if (string.IsNullOrWhiteSpace(name.Value))
           

    //    Name = name;
    //}

    public void Update(
        BranchName name,
        BranchType branchType,
        Address address,
        ContactInfo contactInfo,
        NetworkInfo networkInfo,
        string? description)
    {
        Name=name;
        Type = branchType;

        Address = address;

        ContactInfo = contactInfo;

        NetworkInfo = networkInfo;
        Description = description;
    }
    public void ChangeCode(string code)

    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Branch code boş ola bilməz");
        }

        Code = code.Trim().ToUpperInvariant();
    }
    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;

        IsMonitoringEnabled = true;

    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;

        IsMonitoringEnabled = false;

    }

    public void EnableMaintenanceMode()
    {
        if (IsInMaintenanceMode)
        {
            return;
        }

        IsInMaintenanceMode = true;
    }

    public void DisableMaintenanceMode()
    {
        if (!IsInMaintenanceMode)
        {
            return;
        }

        IsInMaintenanceMode = false;
    }

    // MONITORING

    public void EnableMonitoring()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException(
                "Inactive branch monitoring enable edilə bilməz");
        }

        if (IsMonitoringEnabled)
        {
            return;
        }

        IsMonitoringEnabled = true;
    }

    public void DisableMonitoring()
    {
        if (!IsMonitoringEnabled)
        {
            return;
        }

        IsMonitoringEnabled = false;
    }
    public void MarkAsDeleted()
    {
        if (IsDeleted)
            return;

        IsDeleted = true;

        IsActive = false;

        IsMonitoringEnabled = false;
    }

    public void UpdateLastSeenAt(DateTime? lastSeenAt)

    {
        LastSeenAt = lastSeenAt;
    }
    private void UpdateBranchStatus()
    {
        if (IsInMaintenanceMode)
        {
            Status = BranchStatus.Maintenance;

            return;
        }

        if (TotalDeviceCount == 0)
        {
            Status = BranchStatus.Unknown;

            return;
        }

        if (OfflineDeviceCount == TotalDeviceCount)
        {
            Status = BranchStatus.Offline;

            return;
        }

        if (DegradedDeviceCount > 0)
        {
            Status = BranchStatus.Degraded;

            return;
        }

        if (OnlineDeviceCount > 0)
        {
            Status = BranchStatus.Online;

            return;
        }

        Status = BranchStatus.Unknown;
    }
    public void UpdateStats(int totalDevices, int onlineDevices, int offlineDevices, int degradedDevices, int alertCount ,int healthScore)
    {
        TotalDeviceCount = totalDevices;

        OnlineDeviceCount = onlineDevices;

        OfflineDeviceCount = offlineDevices;

        DegradedDeviceCount = degradedDevices;

        AlertCount = alertCount;
        HealthScore=healthScore;
        UpdateBranchStatus();
    }
   
}