using AuthServer.Domain.UserRoles;
using AuthServer.Domain.Users.ValueObjects;
using SharedLibrary.Abstractions.Entity;
using SharedLibrary.Security;
using System.Text.Json.Serialization;

namespace AuthServer.Domain.Users;

public sealed class User : Entity
{
    private const int MaxFailedAttempts = 5;
    private const int LockMinutes = 15;

    private User(
        FirstName firstName,
        LastName lastName,
        Email email,
        UserName userName,
        
        Password password)
    {
        SetFirstName(firstName);
        SetLastName(lastName);
        SetEmail(email);
        SetUserName(userName);
        SetPassword(password);

        SetFullName();

       
    }

    private User() { }

    // ================= PROFILE =================

    public FirstName FirstName { get; private set; }
        = default!;

    public LastName LastName { get; private set; }
        = default!;

    public FullName FullName { get; private set; }
        = default!;

    public Email Email { get; private set; }
        = default!;

    public UserName UserName { get; private set; }
        = default!;

    // ================= PASSWORD =================

    [JsonIgnore]
    public Password Password { get; private set; }
        = default!;

    // ================= AUTH =================

    public int FailedLoginAttempts
    { get; private set; }

    public DateTimeOffset? LockoutEnd
    { get; private set; }

    // ================= TFA =================

    public bool TFAStatus
    { get; private set; } = default!;

    public string? TFACodeHash
    { get; private set; }

    public string? PendingTFATokenHash
    { get; private set; }

    public DateTimeOffset? TFAExpiresDate
    { get; private set; }

    public bool TFAIsCompleted
    { get; private set; }

    // ================= RESET PASSWORD =================

    public string? ResetPasswordTokenHash
    { get; private set; }

    public DateTimeOffset? ResetPasswordTokenExpiresAt
    { get; private set; }

    public bool IsResetPasswordCompleted
    { get; private set; }

    // ================= ROLES =================

    public ICollection<UserRole> UserRoles
    { get; private set; }
        = new List<UserRole>();

    // ================= FACTORY =================

    public static User Create(
        FirstName firstName,
        LastName lastName,
        UserName userName,
        Email email,
        Password password)
    {
        return new User(
            firstName,
            lastName,
            email,
            userName,
            password);
    }

    // ================= PROFILE METHODS =================

    public void SetFirstName(
        FirstName firstName)
    {
        FirstName = firstName;

        SetFullName();
    }

    public void SetLastName(
        LastName lastName)
    {
        LastName = lastName;

        SetFullName();
    }

    public void SetEmail(
        Email email)
    {
        Email = email;
    }

    public void SetUserName(
        UserName userName)
    {
        UserName = userName;
    }

    private void SetFullName()
    {
        FullName = new FullName(
            $"{FirstName.Value} {LastName.Value}");
    }

    // ================= PASSWORD METHODS =================

    private void SetPassword(
        Password password)
    {
        Password = password;
    }

    public bool VerifyPassword(
        string password)
    {
        return Password
            .VerifyPasswordHash(password);
    }

    public void ChangePassword(
        Password newPassword)
    {
        Password = newPassword;
    }

    public void ResetPassword(
        Password newPassword)
    {
        Password = newPassword;

        IsResetPasswordCompleted = true;

        ClearResetPasswordToken();
    }

    // ================= LOGIN LOCKOUT =================

    public bool IsLockedOut()
    {
        if (LockoutEnd is null)
        {
            return false;
        }

        if (LockoutEnd <= DateTimeOffset.UtcNow)
        {
            FailedLoginAttempts = 0;

            LockoutEnd = null;

            return false;
        }

        return true;
    }

    public void RegisterFailedLogin()
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= MaxFailedAttempts)
        {
            LockoutEnd =
                DateTimeOffset.UtcNow
                    .AddMinutes(LockMinutes);
        }
    }

    public void ResetLoginAttempts()
    {
        FailedLoginAttempts = 0;

        LockoutEnd = null;
    }

    // ================= TFA =================

    public (string Code, string PendingToken)
        CreateTFAChallenge()
    {
        var rawCode =
            Random.Shared
                .Next(100000, 999999)
                .ToString();

        var pendingToken =
            Guid.NewGuid()
                .ToString("N");

        TFACodeHash =
            TokenHashHelper.Hash(rawCode);

        PendingTFATokenHash =
            TokenHashHelper.Hash(pendingToken);

        TFAExpiresDate =
            DateTimeOffset.UtcNow
                .AddMinutes(5);

        TFAIsCompleted = false;

        return (rawCode, pendingToken);
    }

    public bool VerifyTFACode(
        string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        if (TFACodeHash is null)
        {
            return false;
        }

        if (TFAExpiresDate is null)
        {
            return false;
        }

        if (TFAExpiresDate <=
            DateTimeOffset.UtcNow)
        {
            return false;
        }

        var hash =
            TokenHashHelper.Hash(code);

        return TFACodeHash == hash;
    }

    public void CompleteTwoFactorAuthentication()
    {
        TFAIsCompleted = true;
    }

    public void ClearPendingTFA()
    {
        TFACodeHash = null;

        PendingTFATokenHash = null;

        TFAExpiresDate = null;

        TFAIsCompleted = false;
    }

    // ================= RESET PASSWORD =================

    public string GenerateResetPasswordToken()
    {
        var rawToken =
            Guid.NewGuid()
                .ToString("N");

        ResetPasswordTokenHash =
            TokenHashHelper.Hash(rawToken);

        ResetPasswordTokenExpiresAt =
            DateTimeOffset.UtcNow
                .AddMinutes(15);

        IsResetPasswordCompleted = false;

        return rawToken;
    }

    public bool IsResetTokenValid(
        string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        if (ResetPasswordTokenHash is null)
        {
            return false;
        }

        if (ResetPasswordTokenExpiresAt is null)
        {
            return false;
        }

        if (ResetPasswordTokenExpiresAt <=
            DateTimeOffset.UtcNow)
        {
            return false;
        }

        if (IsResetPasswordCompleted)
        {
            return false;
        }

        var tokenHash =
            TokenHashHelper.Hash(token);

        return tokenHash ==
               ResetPasswordTokenHash;
    }

    public void ClearResetPasswordToken()
    {
        ResetPasswordTokenHash = null;

        ResetPasswordTokenExpiresAt = null;
    }
}