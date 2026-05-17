using GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SharedLibrary;
using SharedLibrary.Abstractions.Entity;
using SharedLibrary.BaseContext;
using SharedLibrary.Constants;
using NetWorkPassServer.Application.Context;
using NetWorkPassServer.Domain.Alerts;

namespace NetWorkPassServer.Infrastructure.Context;
public sealed class PasswordDbContext(DbContextOptions<PasswordDbContext> options) : BaseDbContext(options),IPasswordDbContext, IUnitOfWork
{
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PasswordDbContext).Assembly);
        modelBuilder.ApplyGlobalFilters();
        base.OnModelCreating(modelBuilder);
    }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<Device> Devices { get; set; }

    public DbSet<Alert> Alerts { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (Database.ProviderName!=null)
        {
            var SystemUserId = new IdentityId(SystemUser.Id);
            var now = DateTimeOffset.UtcNow;
            ApplyAudit(ChangeTracker, SystemUserId, now);


            return await base.SaveChangesAsync(cancellationToken);
        }
        throw new InvalidOperationException("Use SaveChangesAsync(userId)");

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
