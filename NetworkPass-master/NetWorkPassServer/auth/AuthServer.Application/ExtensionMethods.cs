using AuthServer.Domain.Users;
using SharedLibrary.Abstractions;
using SharedLibrary.Abstractions.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Application;
public static class ExtensionMethods
{
    public static IQueryable<EntityWithAuditDto<TEntity>> ApplyAuditDto<TEntity>(this IQueryable<TEntity> entities, IQueryable<User> users)
        where TEntity : Entity
    {
        return from entity in entities
               join createUser in users
                   on entity.CreatedBy equals createUser.Id
               join updatedUser in users
                   on entity.UpdatedBy equals updatedUser.Id into updatedUsers
               from updatedUser in updatedUsers.DefaultIfEmpty()
               select new EntityWithAuditDto<TEntity>
               {
                   Entity = entity,
                   CreatedUser = new AuditUserInfoDto
                   {
                       Id = createUser.Id,
                       UserName = createUser.UserName.Value,
                       Email = createUser.Email.Value
                   },
                   UpdatedUser = updatedUser != null ? new AuditUserInfoDto
                   {
                       Id = updatedUser.Id,
                       UserName = updatedUser.UserName.Value,
                       Email = updatedUser.Email.Value
                   } : null
               };
    }
}
