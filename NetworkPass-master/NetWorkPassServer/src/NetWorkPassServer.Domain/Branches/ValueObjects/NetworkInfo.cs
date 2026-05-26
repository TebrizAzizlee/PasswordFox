

using NetWorkPassServer.Domain.Shared;

namespace NetWorkPassServer.Domain.Branches.ValueObjects;
public sealed record NetworkInfo {
    public string? WanIp { get; init; } = default!;
    public string? Subnet { get; init; }= default!;
    public string? Gateway { get; init; } = default!;
    public string? DnsServer { get; init; } = default!;
    private NetworkInfo() { }
    public NetworkInfo(string? wanIp,string? subNet,string? gateway,string? dnsServer) 
    
    {
        if (string.IsNullOrWhiteSpace(wanIp))
        {
            throw new ArgumentException(
                "WAN IP boş ola bilməz");
        }

     

        if (string.IsNullOrWhiteSpace(subNet))
        {
            throw new ArgumentException(
                "Subnet boş ola bilməz");
        }

        if (!ValidSubnetMasks.Contains(subNet))
        {
            throw new ArgumentException(
                "Subnet mask düzgün deyil");
        }

        if (string.IsNullOrWhiteSpace(gateway))
        {
            throw new ArgumentException(
                "Gateway boş ola bilməz");
        }

      

        if (string.IsNullOrWhiteSpace(dnsServer))
        {
            throw new ArgumentException(
                "DNS Server boş ola bilməz");
        }

       
        WanIp = wanIp.Trim();
        Subnet = subNet.Trim();
        Gateway = gateway.Trim();
        DnsServer = dnsServer.Trim();
    
    }
    private static readonly HashSet<string>
   ValidSubnetMasks =
[
   "128.0.0.0",
    "192.0.0.0",
    "224.0.0.0",
    "240.0.0.0",
    "248.0.0.0",
    "252.0.0.0",
    "254.0.0.0",
    "255.0.0.0",
    "255.128.0.0",
    "255.192.0.0",
    "255.224.0.0",
    "255.240.0.0",
    "255.248.0.0",
    "255.252.0.0",
    "255.254.0.0",
    "255.255.0.0",
    "255.255.128.0",
    "255.255.192.0",
    "255.255.224.0",
    "255.255.240.0",
    "255.255.248.0",
    "255.255.252.0",
    "255.255.254.0",
    "255.255.255.0",
    "255.255.255.128",
    "255.255.255.192",
    "255.255.255.224",
    "255.255.255.240",
    "255.255.255.248",
    "255.255.255.252"
];
}