using GenericRepository;
using SharedLibrary.Abstractions;
using SharedLibrary.Abstractions.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Domain;
public interface IAuditableRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    IQueryable<EntityWithAuditDto<TEntity>> GetAllWithAudit();
}