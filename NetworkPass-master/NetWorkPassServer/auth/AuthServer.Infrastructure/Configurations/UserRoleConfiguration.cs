using AuthServer.Domain.UserRoles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedLibrary.Abstractions.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Infrastructure.Configurations;
public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                x => x.Value,
                v => new IdentityId(v));

        builder.Property(x => x.UserId)
            .HasConversion(
                x => x.Value,
                v => new IdentityId(v));

        builder.Property(x => x.RoleId)
            .HasConversion(
                x => x.Value,
                v => new IdentityId(v));

        builder.HasIndex(x => new
        {
            x.UserId,
            x.RoleId
        }).IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
      
    }
}
