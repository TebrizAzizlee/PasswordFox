using AuthServer.Application.Dtos;
using AuthServer.Application.Services;
using AuthServer.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;


namespace AuthServer.Infrastructure.Services;
public sealed class TokenService(IOptions<CustomTokenOptions> options) : ITokenService
{
    private readonly CustomTokenOptions customTokenOptions = options.Value;
    private static List<Claim> GetClaims(User user)
    {
        var userList = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier,user.Id.Value.ToString()),
            new("userId", user.Id.Value.ToString()),
            new (JwtRegisteredClaimNames.Email,user.Email.Value),
            new (ClaimTypes.Name,user.UserName.Value),
            new(ClaimTypes.Role, "User"), // 🔥 gələcək üçün vacib
            
        };
        //if (audiences != null)
        //    userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
        return userList;
    }
    public string CreateRefreshToken()
    {
        var numberByte = new byte[32];
        using var rnd = RandomNumberGenerator.Create();
        rnd.GetBytes(numberByte);
        var tokenString = Base64UrlEncoder.Encode(numberByte);
        return tokenString;
    }
    
    public  Task<TokenDto> GenerateTokenAsync(User user, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        Console.WriteLine("AUTH UTC NOW: " + now); // 🔥 BURAYA QOY
        var accessTokenExpiration = now.AddMinutes(customTokenOptions.AccessTokenExpiration);
        var refreshTokenExpiration = now.AddDays(customTokenOptions.RefreshTokenExpiration);
        var securityKey = SignService.GetSymmetricSecurityKey(customTokenOptions.SecurityKey);
        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken jwtSecurityToken = new(
            issuer: customTokenOptions.Issuer,
            audience: customTokenOptions.Audience[0],
            expires: accessTokenExpiration,
            notBefore: now.AddMinutes(-2),
            claims: GetClaims(user).Append(new Claim(JwtRegisteredClaimNames.Jti,Guid.CreateVersion7().ToString())),
            signingCredentials: signingCredentials
            );
        var handler = new JwtSecurityTokenHandler();
        var token = handler.WriteToken(jwtSecurityToken);

        // Refresh token yarat
        var refreshToken = CreateRefreshToken();

        // TokenDto yarat

        var tokenDto = new TokenDto(
            AccessToken: token,
            AccessTokenExpiration: accessTokenExpiration,
            RefreshToken: refreshToken,
            RefreshTokenExpiration: refreshTokenExpiration
        );
        
        return Task.FromResult(tokenDto);
    }
}
