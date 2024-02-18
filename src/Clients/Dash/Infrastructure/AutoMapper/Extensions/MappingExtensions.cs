using Clients.Dash.Models;
using Shared.Common;
using System;
using System.Collections.Generic;

namespace Clients.Dash.Infrastructure.AutoMapper.Extensions;

/// <summary>
/// Represents the extensions to map entity to model and vise versa
/// </summary>
public static class MappingExtensions
{
   #region Utilities

   /// <summary>
   /// Execute a mapping from the source object to a new destination object. The source type is inferred from the source object
   /// </summary>
   /// <typeparam name="TDestination">Destination object type</typeparam>
   /// <param name="source">Source object to map from</param>
   /// <returns>Mapped destination object</returns>
   private static TDestination Map<TDestination>(this object source)
   {
      //use AutoMapper for mapping objects
      return AutoMapperConfiguration.Mapper.Map<TDestination>(source);
   }

   /// <summary>
   /// Execute a mapping from the source object to the existing destination object
   /// </summary>
   /// <typeparam name="TSource">Source object type</typeparam>
   /// <typeparam name="TDestination">Destination object type</typeparam>
   /// <param name="source">Source object to map from</param>
   /// <param name="destination">Destination object to map into</param>
   /// <returns>Mapped destination object, same instance as the passed destination object</returns>
   private static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
   {
      //use AutoMapper for mapping objects
      return AutoMapperConfiguration.Mapper.Map(source, destination);
   }

   #endregion

   #region Methods

   #region Model-Entity mapping

   /// <summary>
   /// Execute a mapping from the entity to a new model
   /// </summary>
   /// <typeparam name="TModel">Model type</typeparam>
   /// <param name="entity">Entity to map from</param>
   /// <returns>Mapped model</returns>
   public static TModel ToModel<TModel>(this BaseEntity entity) where TModel : BaseEntityModel
   {
      if (entity == null)
         throw new ArgumentNullException(nameof(entity));

      return entity.Map<TModel>();
   }


   /// <summary>
   /// Execute a mapping from the entity collection to a new model collection
   /// </summary>
   /// <typeparam name="TModel">Model type</typeparam>
   /// <param name="entity">Entity to map from</param>
   /// <returns>Mapped model</returns>
   public static IEnumerable<TModel> ToModel<TModel>(this IEnumerable<BaseEntity> entity) where TModel : BaseEntityModel
   {
      if (entity == null)
         throw new ArgumentNullException(nameof(entity));

      return entity.Map<IEnumerable<TModel>>();
   }


   /// <summary>
   /// Execute a mapping from the entity to the existing model
   /// </summary>
   /// <typeparam name="TEntity">Entity type</typeparam>
   /// <typeparam name="TModel">Model type</typeparam>
   /// <param name="entity">Entity to map from</param>
   /// <param name="model">Model to map into</param>
   /// <returns>Mapped model</returns>
   public static TModel ToModel<TEntity, TModel>(this TEntity entity, TModel model)
       where TEntity : BaseEntity where TModel : BaseEntityModel
   {
      if (entity == null)
         throw new ArgumentNullException(nameof(entity));

      if (model == null)
         throw new ArgumentNullException(nameof(model));

      return entity.MapTo(model);
   }


   /// <summary>
   /// Execute a mapping from the entity collection to the existing model collection
   /// </summary>
   /// <typeparam name="TEntity">Entity type</typeparam>
   /// <typeparam name="TModel">Model type</typeparam>
   /// <param name="entity">Entity to map from</param>
   /// <param name="model">Model to map into</param>
   /// <returns>Mapped model</returns>
   public static IEnumerable<TModel> ToModel<TEntity, TModel>(this IEnumerable<TEntity> entity, IEnumerable<TModel> model)
       where TEntity : BaseEntity where TModel : BaseEntityModel
   {
      if (entity == null)
         throw new ArgumentNullException(nameof(entity));

      if (model == null)
         throw new ArgumentNullException(nameof(model));

      return entity.MapTo(model);
   }


   /// <summary>
   /// Execute a mapping from the model to a new entity
   /// </summary>
   /// <typeparam name="TEntity">Entity type</typeparam>
   /// <param name="model">Model to map from</param>
   /// <returns>Mapped entity</returns>
   public static TEntity ToEntity<TEntity>(this BaseEntityModel model) where TEntity : BaseEntity
   {
      if (model == null)
         throw new ArgumentNullException(nameof(model));

      return model.Map<TEntity>();
   }


   /// <summary>
   /// Execute a mapping from the model collection to a new entity collection
   /// </summary>
   /// <typeparam name="TEntity">Entity type</typeparam>
   /// <param name="model">Model to map from</param>
   /// <returns>Mapped entity colection</returns>
   public static IEnumerable<TEntity> ToEntity<TEntity>(this IEnumerable<BaseEntityModel> model) where TEntity : BaseEntity
   {
      if (model == null)
         throw new ArgumentNullException(nameof(model));

      return model.Map<IEnumerable<TEntity>>();
   }


   /// <summary>
   /// Execute a mapping from the model to the existing entity
   /// </summary>
   /// <typeparam name="TEntity">Entity type</typeparam>
   /// <typeparam name="TModel">Model type</typeparam>
   /// <param name="model">Model to map from</param>
   /// <param name="entity">Entity to map into</param>
   /// <returns>Mapped entity</returns>
   public static TEntity ToEntity<TEntity, TModel>(this TModel model, TEntity entity)
       where TEntity : BaseEntity where TModel : BaseEntityModel
   {
      if (model == null)
         throw new ArgumentNullException(nameof(model));

      if (entity == null)
         throw new ArgumentNullException(nameof(entity));

      return model.MapTo(entity);
   }


   /// <summary>
   /// Execute a mapping from the model collection to the existing entity collection
   /// </summary>
   /// <typeparam name="TEntity">Entity type</typeparam>
   /// <typeparam name="TModel">Model type</typeparam>
   /// <param name="model">Model to map from</param>
   /// <param name="entity">Entities to map into</param>
   /// <returns>Mapped entity</returns>
   public static IEnumerable<TEntity> ToEntity<TEntity, TModel>(this IEnumerable<TModel> model, IEnumerable<TEntity> entity)
       where TEntity : BaseEntity where TModel : BaseEntityModel
   {
      if (model == null)
         throw new ArgumentNullException(nameof(model));

      if (entity == null)
         throw new ArgumentNullException(nameof(entity));

      return model.MapTo(entity);
   }

   #endregion

   #endregion
}