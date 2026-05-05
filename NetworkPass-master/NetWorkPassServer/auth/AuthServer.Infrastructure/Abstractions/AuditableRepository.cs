using AuthServer.Domain;
using AuthServer.Domain.Users;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Abstractions;
using SharedLibrary.Abstractions.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Infrastructure.Abstractions;
internal class AuditableRepository<TEntity, TContext>(TContext context) : Repository<TEntity, TContext>(context), IAuditableRepository<TEntity>
    where TEntity : Entity
   where TContext : DbContext

{
    private readonly TContext _context = context;

    public IQueryable<EntityWithAuditDto<TEntity>> GetAllWithAudit()
    {
        var entities = _context.Set<TEntity>().AsQueryable();
        var users = _context.Set<User>().AsNoTracking().AsQueryable();

        var res = entities
          .Join(users, m => m.CreatedBy, m => m.Id, (b, user) =>
                  new { entity = b, createdUser = user })
          .GroupJoin(users, m => m.entity.UpdatedBy, m => m.Id, (b, user) =>
                  new { b.entity, b.createdUser, updatedUser = user })
          .SelectMany(s => s.updatedUser.DefaultIfEmpty(),
              (x, updatedUser) => new EntityWithAuditDto<TEntity>
              {
                  Entity = x.entity,
                  CreatedUser =new AuditUserInfoDto
                  {
                      Id=x.createdUser.Id.Value,
                      UserName=x.createdUser.UserName.Value
                  },
                  UpdatedUser =updatedUser==null?null: new AuditUserInfoDto
                  {
                      Id=updatedUser.Id.Value,
                      UserName=updatedUser.UserName.Value
                  }
              });

        return res;
    }
}
