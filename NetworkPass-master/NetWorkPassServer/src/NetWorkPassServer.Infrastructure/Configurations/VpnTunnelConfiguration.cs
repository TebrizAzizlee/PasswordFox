using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetWorkPassServer.Domain.VpnTunnels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Infrastructure.Configurations;
internal sealed class VpnTunnelConfiguration
    : IEntityTypeConfiguration<VpnTunnel>
{
    public void Configure(
        EntityTypeBuilder<VpnTunnel> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TunnelName)
            .HasMaxLength(100);

        builder.OwnsOne(
            x => x.RemoteIpAddress,
            ip =>
            {
                ip.Property(x => x.Value)
                    .HasColumnName("RemoteIpAddress")
                    .HasMaxLength(50)
                    .IsRequired();
            });
    }
}