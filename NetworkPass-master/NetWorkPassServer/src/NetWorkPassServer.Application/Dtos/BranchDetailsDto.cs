using NetWorkPassServer.Domain.Branches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Dtos;
public sealed record BranchDetailsDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string Code { get; set; } = default!;

    public string? Description { get; set; }

    public BranchType Type { get; set; }

    public BranchStatus Status { get; set; }

    // ADDRESS

    public string City { get; set; } = default!;

    public string District { get; set; } = default!;

    public string FullAddress { get; set; } = default!;


    // CONTACT

    public string PhoneNumber1 { get; set; } = default!;

    public string? PhoneNumber2 { get; set; }

    public string Email { get; set; } = default!;


    // NETWORK

    public string WanIp { get; set; } = default!;

    public string Subnet { get; set; } = default!;

    public string Gateway { get; set; } = default!;

    public string DnsServer { get; set; } = default!;


    // STATS

    public int TotalDeviceCount { get; set; }

    public int OnlineDeviceCount { get; set; }

    public int OfflineDeviceCount { get; set; }

    public int WarningDeviceCount { get; set; }

    public int AlertCount { get; set; }


    // STATE

    public DateTime? LastSeenAt { get; set; }

    public bool IsMonitoringEnabled { get; set; }

    public bool IsActive { get; set; }


    // AUDIT

    public DateTime CreationTime { get; set; }

    public DateTime? LastModificationTime { get; set; }
}
