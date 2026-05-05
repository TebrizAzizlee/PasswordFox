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
            return ServiceResult<LoginResult>.Failure("AuthenticationFailed", "İstifadəçi adı və ya şifrə düzgün deyil.", HttpStatusCode.Unauthorized);
        }

        user.ResetLoginAttempts();

        if (!user.TFAStatus.Value)
        {
            var token = await _tokenService.GenerateTokenAsync(user, cancellationToken);
            // köhnələri deactivate et
            var loginTokens = await _loginTokenRepository
                .Where(x => x.UserId == user.Id && x.IsActive)
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.IsActive, false), 
                cancellationToken);

            await SaveRefreshTokenAsync(user, token, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<LoginResult>.Success(new LoginResult
            {

                RequiresTFA = false,
                Token=token

            });
        }
        else
        {
            user.CreateTFACode();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _emailService.SendAsync(
                user.Email.Value,
                "Giriş təsdiqi",
                $"Salam {user.UserName.Value},\nSizin TFA kodunuz: {user.TFAConfirmCode!.Value}\nƏgər bu əməliyyatı siz etməmisinizsə, nəzərə almayın.",
                cancellationToken
            );

            return ServiceResult<LoginResult>.Success(new LoginResult
            {

                RequiresTFA = true
            });
        }
    }
  

    // 🔹 Enterprise level: Save refresh token
    private async Task SaveRefreshTokenAsync(
     User user,
     TokenDto token,
     CancellationToken cancellationToken)
    {
        if (token.RefreshToken is null || token.RefreshTokenExpiration is null)
            throw new Exception("Token is invalid");

        // 🔥 HASH ET
        var hash = TokenHashHelper.Hash(token.RefreshToken);

        var refreshToken = new LoginToken(
            hash,
            user.Id,
            token.RefreshTokenExpiration.Value
        );

        await _loginTokenRepository.AddAsync(refreshToken, cancellationToken);
    }
    // 🔹 Refresh token ilə yeni access token
    public async Task<ServiceResult<TokenDto>> CreateTokenByRefreshTokenAsync(
     string refreshToken,
     CancellationToken ct)
    {
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
          if (existingToken.IsExpired())
        {
            return ServiceResult<TokenDto>.Failure(
                "ExpiredToken",
                "Refresh token expired",
                HttpStatusCode.Unauthorized);
        }

        // 🔥 ATOMIC ROTATION (RACE CONDITION FIX)
        var deactivated = await _loginTokenRepository
            .TryDeactivateAsync(hash, ct);
        // 🔥 REUSE DETECTION (CRITICAL)
        if (!deactivated)
        {
            var allTokens = await _loginTokenRepository
                .GetAllByUserIdAsync(existingToken.UserId, ct);

            foreach (var t in allTokens)
                t.Deactivate("reuse-detected");

            await _unitOfWork.SaveChangesAsync(ct);

            return ServiceResult<TokenDto>.Failure(
                "ReuseDetected",
                "Session compromised. All sessions revoked.",
                HttpStatusCode.Unauthorized);
        }

      

        var user = await _userRepository
            .GetByIdAsync(existingToken.UserId, ct);

        if (user is null)
        {
            return ServiceResult<TokenDto>.Failure(
                "UserNotFound",
                "User not found",
                HttpStatusCode.NotFound);
        }

        // 🔥 ROTATION
       // existingToken.Deactivate("rotated");

        var newToken = await _tokenService.GenerateTokenAsync(user, ct);

        var newHash = TokenHashHelper.Hash(newToken.RefreshToken!);

        var newRefreshToken = new LoginToken(
            newHash,
            user.Id,
            newToken.RefreshTokenExpiration!.Value
        );

        await _loginTokenRepository.AddAsync(newRefreshToken, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return ServiceResult<TokenDto>.Success(newToken);
    }

    // 🔹 Logout / Revoke refresh token
    public async Task<ServiceResult<string>> RevokeRefreshTokenAsync(
      string refreshToken,
      CancellationToken ct)
    {
        var hash = TokenHashHelper.Hash(refreshToken);

        var existing = await _loginTokenRepository
            .GetByRefreshTokenAsync(hash, ct);

        if (existing is null)
            return ServiceResult<string>.Success("Already revoked");

        existing.Deactivate("logout");

        await _unitOfWork.SaveChangesAsync(ct);

        return ServiceResult<string>.Success("Revoked");
    }
}
