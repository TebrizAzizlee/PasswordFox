using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Abstractions.Entity;
public abstract class Entity
{
    protected Entity()
    {
        Id=new IdentityId(Guid.CreateVersion7());
        IsActive=true;
    }
    public IdentityId Id { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public IdentityId CreatedBy { get; private set; } = default!;
    public DateTimeOffset? UpdatedAt { get; private set; }
    public IdentityId? UpdatedBy { get; private set; }



    //Soft Delete dir
    public DateTimeOffset? DeletedAt { get; private set; }
    public IdentityId? DeletedBy { get; private set; }
    public bool IsDeleted { get; private set; }



    public void SetStatus(bool isActive)
    {
        IsActive = isActive;
    }
    public void Delete(IdentityId userId, DateTimeOffset now)
    {
        if (IsDeleted) return;

        SetDeleted(userId, now);
    }

    // Audit Set Methods
    public void SetCreated(IdentityId userId, DateTimeOffset now)
    {
        CreatedBy = userId;
        CreatedAt = now;
    }

    public void SetUpdated(IdentityId userId, DateTimeOffset now)
    {
        UpdatedBy = userId;
        UpdatedAt = now;
    }

    public void SetDeleted(IdentityId userId, DateTimeOffset now)
    {
        DeletedBy = userId;
        DeletedAt = now;
        IsDeleted = true;
    }
}



public sealed record IdentityId(Guid Value)
{
    public static implicit operator Guid(IdentityId id)
    {
        return id.Value;
    }
    public static implicit operator string(IdentityId id)
    {
        return id.Value.ToString();
    }



};
