namespace NetWorkPassServer.Domain.Branches.ValueObjects;

public sealed record Name
{
    public string Value { get; }
    public Name(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException("Name boş ola bilməz");
        }
        if(value.Length>100)
        {
            throw new ArgumentException("Name çox uzundur");

        }
        Value=value.Trim();
    }
    public override string ToString()=>Value;
}