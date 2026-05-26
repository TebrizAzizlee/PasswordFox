

namespace NetWorkPassServer.Domain.Shared;
public sealed record IpAddress
{
    public string Value { get; }

    public IpAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "IP address boş ola bilməz");
        }

        if (!System.Net.IPAddress.TryParse(
            value,
            out _))
        {
            throw new ArgumentException(
                "IP address düzgün deyil");
        }

        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }
}
