using AuthServer.Domain.Permissions;
using AuthServer.Domain.Permissions.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedLibrary.Abstractions.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Infrastructure.Configurations;
internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                x => x.Value,
                x => new IdentityId(x));

        // 🔥 PermissionName VO Mapping

        builder.Property(x => x.Name)
            .HasConversion(
                x => x.Value,
                x => new PermissionName(x))
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Description)
            .HasMaxLength(512);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired();
    }
}

