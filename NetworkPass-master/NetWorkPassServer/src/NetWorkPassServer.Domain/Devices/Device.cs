using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Branches.ValueObjects;
using SharedLibrary.Abstractions.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Domain.Devices;
public sealed partial class Device:Entity
{
    public IdentityId BranchId { get; private set; } = default!;
    public DeviceName Name { get; private set; } = default!;
    public IpAddress Ip_Address { get; private set; } = default!;
    public DeviceType Type { get; private set; }
    public string? Description { get; private set; }
    public Branch Branch { get; private set; } = default!;
    private Device() { }

    public Device(IdentityId branchId, DeviceName name, IpAddress ipAddress, DeviceType type, string? description)
    {
        BranchId = branchId;
        Name = name;
        Ip_Address = ipAddress;
        Type = type;
        Description = description;
    }
    public void Update(
       DeviceName name,
       IpAddress ipAddress,
       DeviceType type,
       string? description)
    {
        Name = name;
        Ip_Address = ipAddress;
        Type = type;
        Description = description;
    }

    public enum DeviceType
    {
        Router,
        AccessPoint,
        Switch,
        Server
    }
}
