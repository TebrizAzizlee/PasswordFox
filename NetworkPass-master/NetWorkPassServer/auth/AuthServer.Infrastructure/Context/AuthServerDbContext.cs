

using AuthServer.Domain.Permissions;
using AuthServer.Domain.RolePermissions;
using AuthServer.Domain.Roles;
using AuthServer.Domain.UserRoles;
using AuthServer.Domain.Users;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SharedLibrary;
using SharedLibrary.Abstractions.Entity;
using SharedLibrary.BaseContext;
using SharedLibrary.Constants;

namespace AuthServer.Infrastructure.Context;
public sealed class AuthServerDbContext(DbContextOptions<AuthServerDbContext> options) : BaseDbContext(options),IUnitOfWork

{

    public DbSet<User> Users  => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<LoginToken> LoginTokens => Set<LoginToken>();
    public DbSet<Role>Roles => Set<Role>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthServerDbContext).Assembly);
        modelBuilder.ApplyGlobalFilters();
        base.OnModelCreating(modelBuilder);
    }

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
    public  async Task<int> SaveChangesAsync(IdentityId userId, CancellationToken cancellationToken = default)
    {
       
       
            var now = DateTimeOffset.UtcNow; 
            ApplyAudit(ChangeTracker, userId, now);


            return await base.SaveChangesAsync(cancellationToken);
       
      



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
