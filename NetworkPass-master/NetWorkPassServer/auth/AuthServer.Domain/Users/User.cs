using AuthServer.Domain.Users.ValueObjects;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Abstractions.Entity;
using SharedLibrary.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace AuthServer.Domain.Users;
public sealed class  User: Entity
{
    public User(FirstName firstName, LastName lastName, Email email, IsAdmin isAdmin, UserName userName, Password password)
    {
        SetFirstName(firstName);
        SetLastName(lastName);
        SetEmail(email);
        SetUserName(userName);
       SetPassword(password);
        SetIsAdmin(isAdmin);
        SetFullName();
        SetTFAStatus(new(false));
    }
    private User() { }

    public FirstName FirstName { get; private set; } = default!;
    public LastName LastName { get; private set; } = default!;
    public FullName FullName { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public UserName UserName { get; private set; } = default!;
    [JsonIgnore]
    public Password Password { get; private set; } = default!;
    
    public IsAdmin Isadmin { get; private set; } = default!;
    public TFACode? TFACode { get; private set; } = default!;
     public TFAConfirmCode? TFAConfirmCode { get;private set; } = default!;
    public TFAStatus TFAStatus { get; private set; } = default!;
    public TFAExpiresDate? TFAExpiresDate { get; private set; } = default!;
    public TFAIsCompleted? TFAIsCompleted { get; private set; } = default!;
    public int FailedLoginAttempts { get; private set; }

    public DateTimeOffset? LockoutEnd { get; private set; }

    private const int MaxFailedAttempts = 5;
    private const int LockMinutes = 15;


    //IsLockedOut() metodu
    public bool IsLockedOut()
    {
        if (LockoutEnd is null)
            return false;

        if (LockoutEnd <= DateTimeOffset.UtcNow)
        {
            FailedLoginAttempts = 0;
            LockoutEnd = null;
            return false;
        }

        return true;
    }
    //Failed login davranışı
    public void RegisterFailedLogin()
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= MaxFailedAttempts)
        {
            LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(LockMinutes);
        }
    }


    //Uğurlu login reset

    public void ResetLoginAttempts()
    {
        FailedLoginAttempts = 0;
        LockoutEnd = null;
    }
    public void SetFirstName(FirstName firstName)
    {
        FirstName=firstName;
    }
    public void SetLastName(LastName lastName)
    {
        LastName=lastName;
    }
    public void SetEmail(Email email)
    {
        Email=email;
    }
    public void SetUserName(UserName userName)
    {
        UserName=userName;
    }
    public void SetIsAdmin(IsAdmin isAdmin)
    {
        Isadmin=isAdmin;
    }
    public void SetPassword(Password password)
    {
        Password=password;
    }
    public void SetTFAStatus(TFAStatus tFAStatus)
    {
        TFAStatus=tFAStatus;
    }
    public string CreateTFACode()
    {
        var code = Random.Shared.Next(100000, 999999).ToString();
        var confirmCode = Guid.NewGuid().ToString("N");

        TFACode = new TFACode(code);
        TFAConfirmCode = new TFAConfirmCode(confirmCode);
        TFAExpiresDate = new TFAExpiresDate(DateTimeOffset.UtcNow.AddMinutes(5));
        TFAIsCompleted = new TFAIsCompleted(false);

        return confirmCode;
    }
    public void SetTFACompleted()
    {
        TFAIsCompleted=new(true);
    }

    // ================= PASSWORD =================

    public bool VerifyPassword(string password)
    {
        return Password.VerifyPasswordHash(password);
    }

    public void ChangePassword(string newPassword)
    {
        Password = new Password(newPassword);
        IsResetPasswordCompleted = true;
        ClearResetPasswordToken();
    }
    public void SetFullName()
    {
        FullName=new FullName(FirstName.Value+ " "+LastName.Value);
    }
   



    // ✅ Reset Token sahələri
    public string? ResetPasswordTokenHash { get; private set; }
    public DateTime? ResetPasswordTokenExpiresAt { get; private set; }
    public bool IsResetPasswordCompleted { get; private set; } = default!;


    public string GenerateResetPasswordToken()
    {
        var token = Guid.NewGuid().ToString("N");

        ResetPasswordTokenHash = TokenHashHelper.Hash(token);
        ResetPasswordTokenExpiresAt = DateTime.UtcNow.AddMinutes(15);
        IsResetPasswordCompleted = false;

        return token;
    }


    public bool IsResetTokenValid(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        if (ResetPasswordTokenHash is null)
            return false;

        var tokenHash = TokenHashHelper.Hash(token);

        return ResetPasswordTokenHash == tokenHash
            && ResetPasswordTokenExpiresAt > DateTime.UtcNow
            && !IsResetPasswordCompleted;
    }

    public void ClearResetPasswordToken()
    {
        ResetPasswordTokenHash = null;
        ResetPasswordTokenExpiresAt = null;
    }

    // ✅ Yeni şifrə təyin etmə
    public void ResetPassword(string newPassword)
    {
        Password = new Password(newPassword);
        ClearResetPasswordToken();
    }
}
