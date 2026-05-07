using AuthServer.Application.Dtos;
using AuthServer.Application.Services;
using AuthServer.Domain.LoginTokens;
using AuthServer.Domain.Users;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using SharedLibrary.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Infrastructure.Services;
public sealed class AuthenticationService(
    ITokenService tokenService,
    ILoginTokenRepository loginTokenRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IEmailService emailService
) : IAuthenticationService
{
    private readonly ITokenService _tokenService = tokenService;
    private readonly ILoginTokenRepository _loginTokenRepository = loginTokenRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;

    // 🔹 Create Token
    public async Task<ServiceResult<LoginResult>> CreateTokenAsync(
        string userName, string password, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FirstOrDefaultAsync(
            x => x.UserName.Value == userName, cancellationToken);
       

        if (user is null)
            return ServiceResult<LoginResult>.Failure("AuthenticationFailed", "İstifadəçi adı və ya şifrə düzgün deyil.", HttpStatusCode.Unauthorized);

        if (user.IsLockedOut())
            return ServiceResult<LoginResult>.Failure("AccountLocked", "Hesab müvəqqəti bloklanıb.", HttpStatusCode.Forbidden);

        if (!user.VerifyPassword(password))
        {
            user.RegisterFailedLogin();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ServiceResult<LoginResult>.Failure(
                "AuthenticationFailed", 
                "İstifadəçi adı və ya şifrə düzgün deyil.",
                HttpStatusCode.Unauthorized);
        }

        // 🔥 PASSWORD DÜZGÜNDÜRSƏ RESET ET
        user.ResetLoginAttempts();

        // 🔥 TFA FLOW
        if (user.TFAStatus.Value)
        {
            user.CreateTFACode();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _emailService.SendAsync(
                user.Email.Value,
                "Giriş təsdiqi",
                $"""
            Salam {user.UserName.Value},

            Sizin TFA kodunuz:
            {user.TFAConfirmCode!.Value}

            Əgər bu əməliyyatı siz etməmisinizsə nəzərə almayın.
            """,
                cancellationToken);

            return ServiceResult<LoginResult>.Success(
                new LoginResult
                {
                    RequiresTFA = true
                });
        }

        var now = DateTimeOffset.UtcNow;

        // 🔥 KÖHNƏ AKTİV TOKENLƏRİ REVOKE ET
        var activeTokens = await _loginTokenRepository
            .GetActiveByUserIdAsync(user.Id, cancellationToken);

        foreach (var activeToken in activeTokens)
        {
            activeToken.Revoke("new-login", now);
        }

        // 🔥 YENİ SESSION FAMILY
        var familyId = Guid.NewGuid();

        // 🔥 TOKEN YARAT
        var token = await _tokenService
            .GenerateTokenAsync(user, cancellationToken);

        // 🔥 REFRESH TOKEN SAVE
        await SaveRefreshTokenAsync(
            user,
            token,
            familyId,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<LoginResult>.Success(
            new LoginResult
            {
                RequiresTFA = false,
                Token = token
            });
    }
  

    // 🔹 Enterprise level: Save refresh token
    private async Task SaveRefreshTokenAsync(
     User user,
     TokenDto token,
     Guid familyId,
     CancellationToken cancellationToken)
    {
        if (token.RefreshToken is null || token.RefreshTokenExpiration is null)
            throw new Exception("   Refresh Token is invalid");

        // 🔥 HASH ET
        var hash = TokenHashHelper.Hash(token.RefreshToken);

        var refreshToken = new LoginToken(
            tokenHash:hash,
           userId: user.Id,
           tokenFamilyId: familyId,
           expiresAt: token.RefreshTokenExpiration.Value
           
        );

        await _loginTokenRepository.AddAsync(refreshToken, cancellationToken);
    }
    // 🔹 Refresh token ilə yeni access token
    public async Task<ServiceResult<TokenDto>> CreateTokenByRefreshTokenAsync(
     string refreshToken,
     
     CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var hash = TokenHashHelper.Hash(refreshToken);

        var existingToken = await _loginTokenRepository
            .GetByRefreshTokenAsync(hash, ct);

        if (existingToken is null)
        {
            return ServiceResult<TokenDto>.Failure(
                "InvalidToken",
                "Refresh token not found",
                HttpStatusCode.Unauthorized);
        }
          if (existingToken.IsExpired(now))
        {
            await RevokeFamilyTokensAsync(existingToken.TokenFamilyId, "expired-reuse",now,ct);

            await _unitOfWork.SaveChangesAsync(ct);

            return ServiceResult<TokenDto>.Failure(
                "ExpiredToken",
                "Refresh token expired",
                HttpStatusCode.Unauthorized);
        }
        // 🔥 REUSE DETECTION
        if (existingToken.IsRevoked())
        {
            await RevokeFamilyTokensAsync(
                existingToken.TokenFamilyId,
                "reuse-detected",
                now,
                ct);
            await _unitOfWork.SaveChangesAsync(ct);


            return ServiceResult<TokenDto>.Failure(
                "ReuseDetected",
                 "Refresh token reuse detected",
                HttpStatusCode.Unauthorized);
        }

      
        //User
        var user = await _userRepository
            .GetByIdAsync(existingToken.UserId, ct);

        if (user is null)
        {
            return ServiceResult<TokenDto>.Failure(
                "UserNotFound",
                "User not found",
                HttpStatusCode.NotFound);
        }

        // 🔥 ROTATE CURRENT TOKEN
         existingToken.Revoke("rotated",now);

        var newToken = await _tokenService.GenerateTokenAsync(user, ct);
        if(newToken.RefreshToken is null || newToken.RefreshTokenExpiration is null)
        {
            return ServiceResult<TokenDto>.Failure(
            "TokenGenerationFailed",
            "Token generation failed",
             HttpStatusCode.InternalServerError);
        }
        // 🔥 HASH NEW REFRESH TOKEN
        var newHash = TokenHashHelper.Hash(newToken.RefreshToken);

        // 🔥 CREATE NEW REFRESH TOKEN NODE
        var newRefreshToken = new LoginToken(
           tokenHash: newHash,
           userId: user.Id,
           tokenFamilyId:existingToken.TokenFamilyId,
           expiresAt:newToken.RefreshTokenExpiration.Value,
            parentTokenId:existingToken.Id
        );
       
        
        await _loginTokenRepository.AddAsync(newRefreshToken, ct);

        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {

            return ServiceResult<TokenDto>.Failure(
         "ConcurrencyFailure",
         "Refresh token already used",
         HttpStatusCode.Unauthorized);
        }

        return ServiceResult<TokenDto>.Success(newToken);
    }

    // 🔹 Logout / Revoke refresh token
    public async Task<ServiceResult<string>> RevokeRefreshTokenAsync(
      string refreshToken,Guid currentUserId,
      CancellationToken ct)
    {
        var hash = TokenHashHelper.Hash(refreshToken);

        var existing = await _loginTokenRepository
            .GetByRefreshTokenAsync(hash, ct);

        // 🔒 token tapılmadı → heç nə demə
        if (existing is null)
            return ServiceResult<string>.Success("OK");
        // 🔒 başqa user tokeni
        if (existing.UserId != currentUserId)
            return ServiceResult<string>.Failure("Forbidden", "Invalid token", HttpStatusCode.Forbidden);
        // 🔒 artıq revoke olunub
        // 🔒 artıq revoke olunub
        if (existing.RevokedAt != null)
        {
            return ServiceResult<string>.Success("OK");
        }
      
       
        // 🔥 atomic revoke
        var revoked = await _loginTokenRepository
            .TryRevokeAsync(hash, "logout", ct);
        if (!revoked)
        {
            return ServiceResult<string>.Success("OK");
        }
        return ServiceResult<string>.Success("OK");
        
      
    }
    private async Task RevokeFamilyTokensAsync(
       Guid familyId,
       string reason,
       DateTimeOffset now,
       CancellationToken ct)
    {
        var familytokens = await _loginTokenRepository
            .Where(x =>
                x.TokenFamilyId == familyId &&
                x.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var token in familytokens)
        {
            token.Revoke(reason, now);
        }
    }

}
