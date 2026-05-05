namespace NetWorkPassServer.Domain.Branches.ValueObjects;


public sealed record Address
{
    public string City { get; }
    public string District { get; }
    public string FullAddress { get; }
    public string PhoneNumber1 { get; }
    public string? PhoneNumber2 { get; }
    public string Email { get; }

    public Address(
      string city,
      string district,
      string fullAddress,
      string phoneNumber1,
      string? phoneNumber2,
      string email)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City boş ola bilməz");

        if (string.IsNullOrWhiteSpace(fullAddress))
            throw new ArgumentException("Address boş ola bilməz");

        if (!IsValidEmail (email))
            throw new ArgumentException("Email düzgün deyil");

        City = city.Trim();
        District = district?.Trim() ?? "";
        FullAddress = fullAddress.Trim();
        PhoneNumber1 = phoneNumber1?.Trim() ?? "";
        PhoneNumber2 = phoneNumber2?.Trim();
        Email = email.Trim().ToLowerInvariant();
    }
    private static bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) && email.Contains("@");
    }
}
       


