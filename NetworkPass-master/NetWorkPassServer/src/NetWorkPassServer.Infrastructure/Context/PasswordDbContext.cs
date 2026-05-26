using GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Domain.Alerts;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.DeviceHeartbeats;
using NetWorkPassServer.Domain.DeviceInterfaces;
using NetWorkPassServer.Domain.DeviceMetricss;
using NetWorkPassServer.Domain.Devices;
using NetWorkPassServer.Domain.InterfaceMetrics;
using NetWorkPassServer.Domain.VpnTunnelHeartbeats;
using NetWorkPassServer.Domain.VpnTunnels;
using SharedLibrary;
using SharedLibrary.Abstractions.Entity;
using SharedLibrary.BaseContext;
using SharedLibrary.Constants;

namespace NetWorkPassServer.Infrastructure.Context;
public sealed class PasswordDbContext(DbContextOptions<PasswordDbContext> options) : BaseDbContext(options),IPasswordDbContext, IUnitOfWork
{
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PasswordDbContext).Assembly);
        modelBuilder.ApplyGlobalFilters();
        base.OnModelCreating(modelBuilder);
    }
    public DbSet<Branch> Branches { get; set; } = default!;
    public DbSet<Device> Devices { get; set; } = default!;
    public DbSet<DeviceHeartbeat>DeviceHeartbeats { get; set; } = default!;
    public DbSet<Alert> Alerts { get; set; } = default!;
    public DbSet<DeviceMetric> DeviceMetrics { get; set; } = default!;
    public DbSet<VpnTunnel> VpnTunnels { get; set; } = default!;
    public DbSet<VpnTunnelHeartbeat> VpnTunnelHeartbeats { get; set; } = default!;
    public DbSet<DeviceInterface> DeviceInterfaces { get; set; }

    public DbSet<InterfaceMetric> InterfaceMetrics { get; set; }

    // EXPLICIT INTERFACE IMPLEMENTATION



    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (Database.ProviderName is null)
        {
            throw new InvalidOperationException(
                "Database provider not configured");
        }
            var SystemUserId = new IdentityId(SystemUser.Id);
            var now = DateTimeOffset.UtcNow;
            ApplyAudit(ChangeTracker, SystemUserId, now);


            return await base.SaveChangesAsync(cancellationToken);
        
        

    }
    public  Task<int> SaveChangesAsync(IdentityId userId, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        ApplyAudit(ChangeTracker,userId, now);

        return base.SaveChangesAsync(cancellationToken);
    }

    internal sealed class IdentityIdValueConverter : ValueConverter<IdentityId, Guid>
    {
        public IdentityIdValueConverter() : base(m => m.Value, m => new IdentityId(m)) { }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<IdentityId>().HaveConversion<IdentityIdValueConverter>();
        // Bütün decimal property-lərə precision tətbiq et
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);

        base.ConfigureConventions(configurationBuilder);
    }
   
    
}
