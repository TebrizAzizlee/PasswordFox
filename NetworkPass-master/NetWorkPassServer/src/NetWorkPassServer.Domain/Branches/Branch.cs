using Abp.Domain.Entities.Auditing;
using NetWorkPassServer.Domain.Alerts;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Branches.ValueObjects;
using NetWorkPassServer.Domain.Devices;
using NetWorkPassServer.Domain.VpnTunnels;

public sealed class Branch : FullAuditedAggregateRoot<Guid>
{
    private Branch()
    {
    }

    public Branch(
        BranchName name,
        Address address,
        ContactInfo contactInfo,
        NetworkInfo networkInfo)
    {
        SetName(name);

        Address = address ?? throw new ArgumentNullException(nameof(address));
        ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
        NetworkInfo = networkInfo ?? throw new ArgumentNullException(nameof(networkInfo));

        Devices = new List<Device>();
        Alerts = new List<Alert>();
        VpnTunnels = new List<VpnTunnel>();

        Status = BranchStatus.Unknown;

        IsActive = true;
        IsMonitoringEnabled = true;
       
    }

    // BASIC

    public BranchName Name { get; private set; } = default!;

    public string Code { get; private set; } = default!;

    public string Description { get; private set; } = default!;
   

    // VALUE OBJECTS

    public Address Address { get; private set; } = default!;

    public ContactInfo ContactInfo { get; private set; } = default!;

    public NetworkInfo NetworkInfo { get; private set; } = default!;


    // STATUS

    public BranchStatus Status { get; private set; } = default!;    

    public BranchType Type { get; private set; }= default!;

    public DateTime? LastSeenAt { get; private set; }

    public bool IsMonitoringEnabled { get; private set; } = default!;

    public bool IsActive { get; private set; } = default!;


    // STATS

    public int OnlineDeviceCount { get; private set; } = default!;

    public int OfflineDeviceCount { get; private set; } = default!;

    public int WarningDeviceCount { get; private set; } = default!;
    public int TotalDeviceCount { get; private set; }
    public int AlertCount { get; private set; } = default!;


    // RELATIONS

    public ICollection<Device> Devices { get; private set; }=default! ;

    public ICollection<Alert> Alerts { get; private set; } = default!;

    public ICollection<VpnTunnel> VpnTunnels { get; private set; } = default!;


    public void SetName(BranchName name)
    {
        if (string.IsNullOrWhiteSpace(name.Value))
            throw new ArgumentException(nameof(name));

        Name = name;
    }

    public void Update(
        BranchName name,
        Address address,
        ContactInfo contactInfo,
        NetworkInfo networkInfo)
    {
        SetName(name);

        Address = address ?? throw new ArgumentNullException(nameof(address));

        ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));

        NetworkInfo = networkInfo ?? throw new ArgumentNullException(nameof(networkInfo));
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
    public void IncreaseDeviceCount()
    {
        TotalDeviceCount++;
    }

    public void DecreaseDeviceCount()
    {
        if (TotalDeviceCount > 0)
            TotalDeviceCount--;
    }
    public void MarkAsDeleted()
    {
        if (IsDeleted)
            return;

        IsDeleted = true;

        IsActive = false;

        IsMonitoringEnabled = false;
    }

    public void RecalculateDeviceStats()
    {
        TotalDeviceCount =
            Devices.Count(x => !x.IsDeleted);

        OnlineDeviceCount =
            Devices.Count(x =>
                !x.IsDeleted &&
                x.Status == DeviceStatus.Online);

        OfflineDeviceCount =
            Devices.Count(x =>
                !x.IsDeleted &&
                x.Status == DeviceStatus.Offline);

        WarningDeviceCount =
            Devices.Count(x =>
                !x.IsDeleted &&
                x.Status == DeviceStatus.Warning);

        UpdateBranchStatus();
    }
    private void UpdateBranchStatus()
    {
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

        if (WarningDeviceCount > 0)
        {
            Status = BranchStatus.Warning;

            return;
        }

        if (OnlineDeviceCount > 0)
        {
            Status = BranchStatus.Online;

            return;
        }

        Status = BranchStatus.Unknown;
    }
}