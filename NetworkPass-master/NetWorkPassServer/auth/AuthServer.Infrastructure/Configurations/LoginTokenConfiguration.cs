using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedLibrary.Abstractions.Entity;

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
       
        
        builder.Property(x => x.Version)
               .IsRowVersion();
        // 🔥 TOKEN HASH
        builder.Property(x => x.TokenHash)
               .IsRequired()
               .HasMaxLength(128);

        // 🔥 INDEX (CRITICAL)
        builder.HasIndex(x => x.TokenHash)
               .IsUnique();
        // 🔥 FAMILY ID
        builder.Property(x => x.TokenFamilyId)
            .IsRequired();
        // 🔥 FAMILY QUERY INDEX
        builder.HasIndex(x => x.TokenFamilyId);
        // 🔥 FAMILY + REVOKED
        builder.HasIndex(x => new
        {
            x.TokenFamilyId,
            x.RevokedAt
        });


        // 🔥 DATES
        builder.Property(x => x.ExpiresAt)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.RevokedAt);

        // 🔥 CREATED DATE
        builder.HasIndex(x => x.CreatedAt);

        // 🔥 EXPIRES DATE
        builder.HasIndex(x => x.ExpiresAt);


        // 🔥 əlavə performans index
        builder.HasIndex(x => x.UserId);
        // 🔥 USER ACTIVE TOKENS
        builder.HasIndex(x => new
        {
            x.UserId,
            x.RevokedAt       
        });
        builder.Property(x => x.ParentTokenId)
    .HasConversion(
        x => x != null ? x.Value : (Guid?)null,
        v => v != null ? new IdentityId(v.Value) : null);
        // 🔥 PARENT TOKEN
        builder.HasIndex(x => x.ParentTokenId)
     .IsUnique()
     .HasFilter("[ParentTokenId] IS NOT NULL");
        builder
    .HasOne(x => x.ParentToken)
    .WithMany()
    .HasForeignKey(x => x.ParentTokenId)
    .OnDelete(DeleteBehavior.Restrict);
    }
}