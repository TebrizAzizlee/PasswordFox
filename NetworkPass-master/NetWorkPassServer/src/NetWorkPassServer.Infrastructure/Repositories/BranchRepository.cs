using GenericRepository;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Infrastructure.Repositories;
internal sealed class BranchRepository(PasswordDbContext context) : Repository<Branch, PasswordDbContext>(context), IBranchRepository
{
}