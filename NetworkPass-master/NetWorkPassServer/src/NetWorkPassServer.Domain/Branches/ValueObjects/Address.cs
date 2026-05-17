namespace NetWorkPassServer.Domain.Branches.ValueObjects;


public sealed record Address
{
    public string City { get; }
    public string District { get; }
    public string FullAddress { get; }

    public Address(
      string city,
      string district,
      string fullAddress
      )
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City boş ola bilməz");

        if (string.IsNullOrWhiteSpace(fullAddress))
            throw new ArgumentException("Address boş ola bilməz");

        

        City = city.Trim();
        District = district?.Trim() ?? "";
        FullAddress = fullAddress.Trim();
       
        
    }
   
}
       


