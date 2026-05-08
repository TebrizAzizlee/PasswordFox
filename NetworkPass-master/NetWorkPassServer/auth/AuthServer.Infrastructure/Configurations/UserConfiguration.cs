using AuthServer.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedLibrary.Consts;
using System.Data;

namespace AuthServer.Infrastructure.Configurations;
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.OwnsOne(x => x.FirstName, fn =>
        {
            fn.Property(p => p.Value)
            .HasColumnType(SqlDbType.NVarChar.ToString())
                .HasColumnName("FirstName")
                .HasMaxLength(EntityConsts.MaxNameLength)
                .IsRequired();
        });
        builder.OwnsOne(x => x.LastName, fn =>
        {
            fn.Property(p => p.Value)
            .HasColumnType(SqlDbType.NVarChar.ToString())
            .HasColumnName("LastName")
            .HasMaxLength(EntityConsts.MaxNameLength)
            .IsRequired();
        });
        builder.OwnsOne(x => x.FullName, fn =>
        {
            fn.Property(p => p.Value)
            .HasColumnType(SqlDbType.NVarChar.ToString())
            .HasColumnName("FullName")
            .HasMaxLength(EntityConsts.MaxFullNameLength);
        });
        builder.OwnsOne(x => x.Email, fn =>
        {
            fn.Property(p => p.Value)
            .HasColumnName("Email")
            .HasMaxLength(EntityConsts.MaxEmailLength)
            .IsRequired();
            fn.HasIndex(p => p.Value).IsUnique();
        });
       


        builder.OwnsOne(x => x.UserName, u =>
        {
            u.Property(p => p.Value)
             .HasColumnName("UserName")
             .HasMaxLength(EntityConsts.MaxNameLength)
             .IsRequired();
            u.HasIndex(p => p.Value).IsUnique();

        });
       

        builder.OwnsOne(x => x.Password, p =>
        {
            p.Property(x => x.PasswordHash)
             .HasColumnName("PasswordHash")
             .HasMaxLength(512)
             .IsRequired();
        });
        builder.Property(x => x.TFAStatus)
    .IsRequired();
        builder.Property(x => x.TFACodeHash)
    .HasMaxLength(256);

        builder.Property(x => x.PendingTFATokenHash)
            .HasMaxLength(256);
        builder.HasIndex(x => x.PendingTFATokenHash).IsUnique()

    .HasFilter("[PendingTFATokenHash] IS NOT NULL");
        builder.Property(x => x.TFAExpiresDate);

        builder.Property(x => x.TFAIsCompleted)
            .IsRequired();

        builder.Property(x => x.ResetPasswordTokenHash)
            .HasMaxLength(256);
        builder.HasIndex(x => x.ResetPasswordTokenHash).IsUnique()

    .HasFilter("[ResetPasswordTokenHash] IS NOT NULL");
        builder.Property(x => x.ResetPasswordTokenExpiresAt);

        builder.Property(x => x.IsResetPasswordCompleted)
            .IsRequired();

       
    }

   
}
