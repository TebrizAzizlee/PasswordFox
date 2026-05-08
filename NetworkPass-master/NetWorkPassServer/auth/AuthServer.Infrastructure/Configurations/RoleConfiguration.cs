using AuthServer.Domain.Roles;
using AuthServer.Domain.Roles.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Infrastructure.Configurations;
public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(x => x.Name)
    .HasConversion(
        x => x.Value,
        v => new RoleName(v))
    .HasMaxLength(64)
    .IsRequired();

    builder.HasIndex(x => x.Name)
    .IsUnique();
    }
}
