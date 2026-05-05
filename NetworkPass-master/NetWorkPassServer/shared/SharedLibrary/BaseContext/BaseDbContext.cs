using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SharedLibrary.Abstractions.Entity;


namespace SharedLibrary.BaseContext;
public abstract class BaseDbContext(DbContextOptions options) : DbContext(options)
{
    protected static void ApplyAudit(ChangeTracker changeTracker, IdentityId userId, DateTimeOffset now)
    {
       

        var entries = changeTracker
            .Entries<Entity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreated(userId, now);
                    break;

                case EntityState.Modified:

                    if (!entry.Entity.IsDeleted && entry.Properties.Any(p => p.IsModified))
                        entry.Entity.SetUpdated(userId, now);
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.SetDeleted(userId, now);
                    break;
            }
        }
    }
}
