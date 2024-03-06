using Hub.Core.Domain.Common;
using Hub.Data.Mapping;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using LinqToDB.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Hub.Data;

/// <summary>
/// Represents the database context for the application
/// </summary>
public class AppDbContext : DbContext, IMappingEntityAccessor
{
   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
   {
      SavingChanges += SavingChanges_SoftDeleteAndModifiedControl;
   }

   #endregion

   #region Event handlers

   /// <summary>
   /// Hadles Soft deleting and modified control before changing save
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   private void SavingChanges_SoftDeleteAndModifiedControl(object sender, SavingChangesEventArgs e)
   {
      var context = (DbContext)sender;

      var modifiedEntities = context.ChangeTracker.Entries()
         .Where(c => c.Entity is ISoftDeletedEntity && c.State is EntityState.Deleted
                || c.Entity is IModifiedEntity && (c.State is EntityState.Added || c.State is EntityState.Modified));

      foreach (var entry in modifiedEntities)
      {
         var dateTime = DateTime.UtcNow;

         if (entry.State is EntityState.Added && entry.Entity is IModifiedEntity)
            entry.Property(nameof(IModifiedEntity.CreatedOnUtc)).CurrentValue = dateTime;

         if (entry.State is EntityState.Deleted && entry.Entity is ISoftDeletedEntity)
         {
            entry.Property(nameof(ISoftDeletedEntity.IsDeleted)).CurrentValue = true;
            entry.State = EntityState.Modified;
         }

         if (entry.State is EntityState.Modified && entry.Entity is IModifiedEntity)
            entry.Property(nameof(IModifiedEntity.UpdatedOnUtc)).CurrentValue = dateTime;
      }
   }

   #endregion

   #region Configuration

   /// <summary>
   /// Configures data context creation
   /// </summary>
   /// <param name="optionsBuilder">OPtion builder</param>
   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
   {
      base.OnConfiguring(optionsBuilder);
   }


   /// <summary>
   /// Further configuration the model
   /// </summary>
   /// <param name="modelBuilder">The builder being used to construct the model for this context</param>
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);

      //dynamically load all entity type configurations
      var typeConfigurations = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes().Where(type => (type.BaseType?.IsGenericType ?? false)
           && type.BaseType.GetGenericTypeDefinition() == typeof(AppEntityTypeConfiguration<>)));

      foreach (var typeConfiguration in typeConfigurations)
      {
         var configuration = (IMappingConfiguration)Activator.CreateInstance(typeConfiguration);
         configuration.ApplyConfiguration(modelBuilder);
      }
   }

   #endregion

   #region Bulk operations

   /// <summary>
   /// Bulk updating entities
   /// </summary>
   /// <param name="entities">Entities</param>
   /// <returns></returns>
   public virtual async Task BulkUpdateAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
   {
      var modEntities = entities.OfType<IModifiedEntity>();
      foreach (var entity in modEntities)
         entity.UpdatedOnUtc = DateTime.UtcNow;

      ChangeTracker.Clear();

      using var dataContext = this.CreateLinqToDBConnection();

      // TODO update for SQLite from Blazor.Orbital example
      //  IsNpgsql only postgress less than 15v
      if (!Database.IsSqlite() && !Database.IsNpgsql())
      {
         await dataContext.GetTable<TEntity>()
                .Merge()
                .Using(entities)
                .OnTargetKey()
                .UpdateWhenMatched()
                .MergeAsync();
      }
      else
         foreach (var entity in entities)
            await dataContext.UpdateAsync(entity);
   }

   /// <summary>
   /// Performs bulk insert operation for entity colllection.
   /// </summary>
   /// <param name="entities">Entities for insert operation</param>
   /// <typeparam name="TEntity">Entity type</typeparam>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task BulkInsertAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
   {
      ChangeTracker?.Clear();

      if (typeof(TEntity).GetInterface(nameof(IModifiedEntity)) is not null)
      {
         var dateTime = DateTime.UtcNow;
         foreach (var entity in entities)
            ((IModifiedEntity)entity).CreatedOnUtc = dateTime;
      }
      using var connection = this.CreateLinqToDBConnection();
      await connection.BulkCopyAsync(new BulkCopyOptions(), entities.RetrieveIdentity(connection));
   }


   // TODO bulk soft delete
   /// <summary>
   /// Performs delete records in a table
   /// </summary>
   /// <param name="entities">Entities for delete operation</param>
   /// <typeparam name="TEntity">Entity type</typeparam>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task BulkDeleteAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
   {
      ChangeTracker?.Clear();

      await using var dataContext = this.CreateLinqToDBConnection();
      if (entities.All(entity => entity.Id == 0))
         foreach (var entity in entities)
            await dataContext.DeleteAsync(entity);
      else
         await dataContext.GetTable<TEntity>()
            .Where(e => e.Id.In(entities.Select(x => x.Id)))
            .DeleteAsync();
   }


   /// <summary>
   /// Performs delete records in a table by a condition
   /// </summary>
   /// <param name="predicate">A function to test each element for a condition.</param>
   /// <typeparam name="TEntity">Entity type</typeparam>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the number of deleted records
   /// </returns>
   public virtual async Task<int> BulkDeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : BaseEntity
   {
      ChangeTracker?.Clear();

      await using var dataContext = this.CreateLinqToDBConnection();
      return await dataContext.GetTable<TEntity>().Where(predicate).DeleteAsync();
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Modify the input SQL query by adding passed parameters
   /// </summary>
   /// <param name="sql">The raw SQL query</param>
   /// <param name="parameters">The values to be assigned to parameters</param>
   /// <returns>Modified raw SQL query</returns>
   protected virtual string CreateSqlWithParameters(string sql, params object[] parameters)
   {
      //add parameters to sql
      for (var i = 0; i <= (parameters?.Length ?? 0) - 1; i++)
      {
         if (parameters[i] is not DbParameter parameter)
            continue;

         sql = $"{sql}{(i > 0 ? "," : string.Empty)} @{parameter.ParameterName}";

         //whether parameter is output
         if (parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Output)
            sql = $"{sql} output";
      }

      return sql;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Creates a DbSet that can be used to query and save instances of entity
   /// </summary>
   /// <typeparam name="TEntity">Entity type</typeparam>
   /// <returns>A set for the given entity type</returns>
   public virtual DbSet<TEntity> GetTable<TEntity>() where TEntity : BaseEntity
   {
      return base.Set<TEntity>();
   }

   /// <summary>
   /// Creates a LINQ query for the entity based on a raw SQL query
   /// </summary>
   /// <typeparam name="TEntity">Entity type</typeparam>
   /// <param name="sql">The raw SQL query</param>
   /// <param name="parameters">The values to be assigned to parameters</param>
   /// <returns>An IQueryable representing the raw SQL query</returns>
   public virtual IQueryable<TEntity> EntityFromSql<TEntity>(string sql, params object[] parameters) where TEntity : BaseEntity
   {
      return this.GetTable<TEntity>().FromSqlRaw(CreateSqlWithParameters(sql, parameters), parameters);
   }

   /// <summary>
   /// Executes the given SQL against the database
   /// </summary>
   /// <param name="sql">The SQL to execute</param>
   /// <param name="doNotEnsureTransaction">true - the transaction creation is not ensured; false - the transaction creation is ensured.</param>
   /// <param name="timeout">The timeout to use for command. Note that the command timeout is distinct from the connection timeout, which is commonly set on the database connection string</param>
   /// <param name="parameters">Parameters to use with the SQL</param>
   /// <returns>The number of rows affected</returns>
   public virtual async Task<int> ExecuteSqlCommandAsync(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
   {
      //set specific command timeout
      var previousTimeout = Database.GetCommandTimeout();
      Database.SetCommandTimeout(timeout);

      var result = 0;
      if (!doNotEnsureTransaction)
      {
         //use with transaction
         using var transaction = Database.BeginTransaction();
         result = await Database.ExecuteSqlRawAsync(sql, parameters);
         await transaction.CommitAsync();
      }
      else
         result = await Database.ExecuteSqlRawAsync(sql, parameters);

      //return previous timeout back
      Database.SetCommandTimeout(previousTimeout);

      return result;
   }

   /// <summary>
   /// Creates a new temporary storage and populate it using data from provided query
   /// </summary>
   /// <param name="storeKey">Name of temporary storage</param>
   /// <param name="query">Query to get records to populate created storage with initial data</param>
   /// <typeparam name="TItem">Storage record mapping class</typeparam>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the iQueryable instance of temporary storage
   /// </returns>
   public virtual Task<ITempDataStorage<TItem>> CreateTempDataStorageAsync<TItem>(string storeKey, IQueryable<TItem> query)
       where TItem : class
   {
      return Task.FromResult<ITempDataStorage<TItem>>(new TempSqlDataStorage<TItem>(storeKey, query, this.CreateLinqToDBContext()));
   }


   /// <summary>
   /// Truncates database table
   /// </summary>
   /// <param name="resetIdentity">Performs reset identity column</param>
   /// <typeparam name="TEntity">Entity type</typeparam>
   public virtual async Task TruncateAsync<TEntity>(bool resetIdentity = false) where TEntity : BaseEntity
   {
      using var dataContext = this.CreateLinqToDBContext();
      await dataContext.GetTable<TEntity>().TruncateAsync(resetIdentity);
   }

   /// <summary>
   /// Returns mapped entity descriptor
   /// </summary>
   /// <param name="entityType">Type of entity</param>
   /// <returns>Mapped entity descriptor</returns>
   /// <exception cref="NotImplementedException"></exception>
   public AppEntityDescriptor GetEntityDescriptor(Type entityType)
   {
      var designModelService = this.GetService<IDesignTimeModel>();
      var model = designModelService.Model;
      var efType = model.FindEntityType(entityType);

      var descriptor = new AppEntityDescriptor()
      {
         EntityName = efType.GetTableName(),
         Fields = efType.GetProperties().Select(column =>
         {
            var mapping = column.GetRelationalTypeMapping();
            var fieldDescriptor = new AppEntityFieldDescriptor()
            {
               Name = column.Name,
               IsPrimaryKey = column.IsPrimaryKey(),
               IsNullable = column.IsNullable,
               IsUnique = column.IsUniqueIndex(),
               Type = mapping.ClrType,
               Size = mapping.Size,
               Precision = mapping.Precision,
               IsIdentity = column.GetIdentityIncrement().HasValue,
            };

            return fieldDescriptor;

         }).ToList()
      };

      return descriptor;
   }


   /// <inheritdoc/>
   public override void Dispose()
   {
      SavingChanges -= SavingChanges_SoftDeleteAndModifiedControl;
      //GC.SuppressFinalize(this);
      base.Dispose();
   }

   #endregion
}