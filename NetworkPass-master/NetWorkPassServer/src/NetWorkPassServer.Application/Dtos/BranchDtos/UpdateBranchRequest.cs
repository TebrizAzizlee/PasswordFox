using NetWorkPassServer.Domain.Branches;

namespace NetWorkPassServer.Application.Dtos.BranchDtos;
public sealed record UpdateBranchRequest(
    string Code,
    string BranchName,
    string City,
    string District,
    string FullAddress,
    string PhoneNumber1,
    string? PhoneNumber2,
    string Email,
    string WanIp,
    string Subnet,
    string Gateway,
    string DnsServer,
    BranchType Type,
    string? Description
);
