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
             .IsRequired();
        });
        builder.OwnsOne(x => x.Isadmin);
        builder.OwnsOne(x => x.TFAStatus);
        builder.OwnsOne(x => x.TFACode);
        builder.OwnsOne(x => x.TFAConfirmCode);
        builder.OwnsOne(x => x.TFAExpiresDate);
        builder.OwnsOne(x => x.TFAIsCompleted);

        builder.HasIndex(x => x.Id);
    }

   
}
