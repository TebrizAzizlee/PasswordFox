using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Configurations;
public sealed class CustomTokenOptions
{
    public IReadOnlyList<String> Audience { get; init; } = default!;
    public string Issuer { get; init; } = default!;
    public int AccessTokenExpiration { get; init; }
    public int RefreshTokenExpiration { get; init; }
    public string SecurityKey { get; init; } = default!;

}
