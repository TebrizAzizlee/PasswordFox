using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetWorkPassServer.Domain.DeviceMetricss;

namespace NetWorkPassServer.Infrastructure.Configurations;

internal sealed class DeviceMetricConfiguration
    : IEntityTypeConfiguration<DeviceMetric>
{
    public void Configure(
        EntityTypeBuilder<DeviceMetric> builder)
    {
        builder.ToTable("DeviceMetrics");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DeviceId)
            .IsRequired();

        builder.Property(x => x.OccurredAtUtc)
            .IsRequired();

        builder.Property(x => x.CpuUsage);

        builder.Property(x => x.MemoryUsage);

        builder.Property(x => x.DiskUsage);

        builder.Property(x => x.Temperature);

        builder.Property(x => x.PingLatency);

        builder.HasIndex(x => x.DeviceId);

        builder.HasIndex(x => x.OccurredAtUtc);

        builder.HasIndex(x =>
            new
            {
                x.DeviceId,
                x.OccurredAtUtc
            });

        builder.HasOne(x => x.Device)
            .WithMany(x => x.Metrics)
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}