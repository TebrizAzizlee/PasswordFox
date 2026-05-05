using Microsoft.EntityFrameworkCore;
using SharedLibrary.Abstractions.Entity;
using System.Linq.Expressions;

namespace SharedLibrary;
public static class ExtensionMethods
{

    //IsDelete olanlari yeni silinen (false olanlari) Entityleri  getirmemek ucun avtomatik cagrilan metoddur...
    public static void ApplyGlobalFilters(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            if (typeof(Entity).IsAssignableFrom(clrType))
            {
                var parameter = Expression.Parameter(clrType, "e");
                var property = Expression.Property(parameter, nameof(Entity.IsDeleted));
                var condition = Expression.Equal(property, Expression.Constant(false));
                var lambda = Expression.Lambda(condition, parameter);

                entityType.SetQueryFilter(lambda);
            }
        }
    }
}
