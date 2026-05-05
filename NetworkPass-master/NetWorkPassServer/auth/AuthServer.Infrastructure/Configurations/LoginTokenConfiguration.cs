using AuthServer.Domain.LoginTokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedLibrary.Abstractions.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Infrastructure.Configurations;
internal sealed class LoginTokenConfiguration : IEntityTypeConfiguration<LoginToken>
{
    public void Configure(EntityTypeBuilder<LoginToken> builder)
    {
        builder.HasKey(x => x.Id);

        // 🔥 IdentityId mapping (əgər ValueObject-dirsə)
        builder.Property(x => x.Id)
               .HasConversion(x => x.Value, v => new IdentityId(v));

        builder.Property(x => x.UserId)
               .HasConversion(x => x.Value, v => new IdentityId(v))
               .IsRequired();

        // 🔥 TOKEN HASH
        builder.Property(x => x.TokenHash)
               .IsRequired()
               .HasMaxLength(500);

        // 🔥 INDEX (CRITICAL)
        builder.HasIndex(x => x.TokenHash)
               .IsUnique();

        // 🔥 STATUS
        builder.Property(x => x.IsActive)
               .IsRequired();

        // 🔥 DATES
        builder.Property(x => x.ExpiresAt)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.RevokedAt);

        // 🔥 əlavə performans index
        builder.HasIndex(x => x.UserId);
    }
}