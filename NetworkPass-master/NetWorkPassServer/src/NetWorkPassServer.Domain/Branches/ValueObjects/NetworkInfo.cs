

namespace NetWorkPassServer.Domain.Branches.ValueObjects;
public sealed record NetworkInfo { 
    public string? WanIp { get; init; }
    public string? Subnet { get; init; }
    public string? Gateway { get; init; }
    public string? DnsServer { get; init; }
    private NetworkInfo() { }
    public NetworkInfo(string? wanIp,string? subNet,string? gateway,string? dnsServer) 
    
    {
        WanIp = wanIp;
        Subnet = subNet;
        Gateway = gateway;
        DnsServer = dnsServer;
    
    }
}