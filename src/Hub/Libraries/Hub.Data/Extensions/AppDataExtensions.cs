using Hub.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Common;
using System;
using System.Linq.Expressions;

namespace Hub.Data.Extensions;

/// <summary>
/// Represents a custom extensions of the application data layer
/// </summary>
public static class AppDataExtensionsSoft
{

   /// <summary>
   /// Configure an index for entity that can be soft deleted
   /// </summary>
   /// <typeparam name="TEntity">Entity</typeparam>
   /// <param name="builder">Entity type builder</param>
   /// <param name="expression">Property index expression</param>
   /// <returns></returns>
   public static IndexBuilder<TEntity> HasSoftIndex<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, object>> expression)
      where TEntity : BaseEntity
   {
      if (typeof(TEntity).GetInterface(nameof(ISoftDeletedEntity)) is not null)
      {
         var dataConfig = DataSettingsManager.LoadSettings();

         return (dataConfig.DataProvider) switch
         {
            DataProviderType.SqlServer => builder.HasIndex(expression).HasFilter($"[{nameof(ISoftDeletedEntity.IsDeleted)}] = 0"),
            DataProviderType.MySql => builder.HasIndex(expression).HasFilter($"[{nameof(ISoftDeletedEntity.IsDeleted)}] = 0"),
            DataProviderType.PostgreSQL => builder.HasIndex(expression).HasFilter($"\"{nameof(ISoftDeletedEntity.IsDeleted)}\" = false"),
            DataProviderType.SQLite => builder.HasIndex(expression).HasFilter($"[{nameof(ISoftDeletedEntity.IsDeleted)}] = 0"),
            DataProviderType.Unknown => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
         };

      }

      return builder.HasIndex(expression);
   }
}