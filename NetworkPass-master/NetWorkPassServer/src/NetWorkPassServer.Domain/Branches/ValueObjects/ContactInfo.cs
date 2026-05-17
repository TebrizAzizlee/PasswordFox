using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Domain.Branches.ValueObjects;
public sealed  record ContactInfo
{
    public string PhoneNumber1 { get; init; } = default!;
    public string? PhoneNumber2 { get; init; }
    public string Email { get; init; }=default!;

    public ContactInfo(string phoneNumber1,string? phoneNumber2,string email)
    {
        if (!IsValidEmail(email))
            throw new ArgumentException("Email düzgün deyil");
        PhoneNumber1 = phoneNumber1?.Trim() ?? "";
        PhoneNumber2 = phoneNumber2?.Trim();
        Email = email.Trim().ToLowerInvariant();
    }

    private static bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) && email.Contains("@");
    }
}
