namespace NetWorkPassServer.Domain.Branches.ValueObjects;

public sealed record BranchName
{
    public string Value { get; }
    public BranchName(string value)
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