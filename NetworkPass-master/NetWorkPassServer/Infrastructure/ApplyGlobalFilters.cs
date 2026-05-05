using Microsoft.EntityFrameworkCore;
using SharedLibrary.Abstractions.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure;
public static class ExtensionMethods
{

    //IsDelete olanlari yeni silinen (false olanlari) Entityleri  getirmemek ucun avtomatik cagrilan metoddur...
    public static void ApplyGlobalFilters(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.IsOwned()) continue;
            var clrType = entityType.ClrType;

            if (!typeof(Entity).IsAssignableFrom(clrType)) continue;

            var parameter = Expression.Parameter(clrType, "e");
            var property = Expression.Property(parameter, nameof(Entity.IsDeleted));
            var condition = Expression.Equal(property, Expression.Constant(false));
            var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(clrType, typeof(bool)), condition, parameter);

            entityType.SetQueryFilter(lambda);

        }
    }
}

