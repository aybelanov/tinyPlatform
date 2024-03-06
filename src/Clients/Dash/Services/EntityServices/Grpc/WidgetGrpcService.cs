using Clients.Dash.Caching;
using Clients.Dash.Domain;
using Clients.Dash.Pages.Configuration.Widgets;
using Clients.Dash.Services.Security;
using Microsoft.Extensions.Caching.Memory;
using Shared.Clients;
using Shared.Clients.Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;

namespace Clients.Dash.Services.EntityServices.Grpc;

/// <summary>
/// Represents the widget presentation service
/// </summary>
/// <remarks>
/// IoC Ctor
/// </remarks>
public class WidgetGrpcService(WidgetRpc.WidgetRpcClient grpcClient,
   IStaticCacheManager staticCacheManager,
   IMemoryCache memoryCache,
   PermissionService permissionService,
   ClearCacheService clearCacheService) : IWidgetService
{
   #region Methods

   #region Get

   /// <summary>
   /// Get all widgets (for admins only) by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>All monitor entity collection</returns>
   public async Task<IFilterableList<Widget>> GetAllWidgetsAsync(DynamicFilter filter)
   {
      var cacheKey = staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Widget>.ByDynamicFilterCacheKey, "all", filter);

      Func<Task<FilterableList<Widget>>> acquire = async () =>
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await grpcClient.GetAllWidgetsAsync(filterProto);

         var widgets = Auto.Mapper.Map<FilterableList<Widget>>(query.Widgets);
         widgets.TotalCount = query.TotalCount ?? 0;

         foreach (var widget in widgets)
         {
            var widgetCacheKey = staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Widget>.ByIdCacheKey, widget.Id);
            await staticCacheManager.SetAsync(widgetCacheKey, widget);
         }

         return widgets;
      };

      return await staticCacheManager.GetAsync(cacheKey, acquire);
   }

   /// <summary>
   /// Gets all widget entities of the current user
   /// </summary>
   /// <returns>Widget collection</returns>
   public async Task<IFilterableList<Widget>> GetUserWidgetsAsync(DynamicFilter filter)
   {
      var cacheKey = staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Widget>.ByDynamicFilterCacheKey, "own", filter);

      async Task<FilterableList<Widget>> acquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await grpcClient.GetOwnWidgetsAsync(filterProto);

         var widgets = Auto.Mapper.Map<FilterableList<Widget>>(query.Widgets);
         widgets.TotalCount = query.TotalCount ?? 0;

         foreach (var widget in widgets)
         {
            var monitorCacheKey = staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Widget>.ByIdCacheKey, widget.Id);
            await staticCacheManager.SetAsync(monitorCacheKey, widget);
         }

         return widgets;
      }

      return await staticCacheManager.GetAsync(cacheKey, acquire);
   }

   /// <summary>
   /// Gets a widget entity by the identifier
   /// </summary>
   /// <param name="id">Identifier</param>
   /// <returns>Widget instance</returns>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task<Widget> GetByIdAsync(long id)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(id, 1);

      var cacheKey = staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Widget>.ByIdCacheKey, id);

      Func<Task<Widget>> acquire = async () =>
      {
         var filter = new DynamicFilter() { Query = $"query => query.Where(x => x.Id == {id}).Take(1)" };
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await permissionService.IsAdminModeAsync()
         ? await grpcClient.GetAllWidgetsAsync(filterProto)
         : await grpcClient.GetOwnWidgetsAsync(filterProto);

         var widget = Auto.Mapper.Map<List<Widget>>(query.Widgets).FirstOrDefault();

         return widget;
      };

      return await staticCacheManager.GetAsync(cacheKey, acquire);
   }

   /// <summary>
   /// Gets widget entities by the identifiers
   /// </summary>
   /// <param name="ids">Identifiers</param>
   /// <returns>Widget collection</returns>
   /// <exception cref="ArgumentNullException"></exception>
   public async Task<IFilterableList<Widget>> GetByIdsAsync(IEnumerable<long> ids)
   {
      ArgumentNullException.ThrowIfNull(ids);

      if (!ids.Any())
         return new FilterableList<Widget>();

      var filter = new DynamicFilter() { Query = $"query => query.Where(x => @0.Contains(x.Id))", Ids = ids.ToArray() };

      var widgets = await GetAllWidgetsAsync(filter);

      foreach (var widget in widgets)
      {
         var cacheKey = staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Widget>.ByIdCacheKey, widget.Id);
         await staticCacheManager.SetAsync(cacheKey, widget);
      }

      return widgets;
   }

   /// <summary>
   /// Gets all device select item list (for the admin mode)
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device select item collection</returns>
   public async Task<IFilterableList<WidgetSelectItem>> GetAllWidgetSelectListAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var cacheKey = staticCacheManager.PrepareKeyForShortTermCache(CacheDefaults<WidgetSelectItem>.ByDynamicFilterCacheKey, "all", filter);
      return await staticCacheManager.GetAsync(cacheKey, acquire);

      async Task<IFilterableList<WidgetSelectItem>> acquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await grpcClient.GetAllWidgetSelectItemsAsync(filterProto);
         var widgets = Auto.Mapper.Map<FilterableList<WidgetSelectItem>>(query.Widgets);
         widgets.TotalCount = query.TotalCount ?? 0;

         return widgets;
      }
   }

   /// <summary>
   /// Gets device select item list for the current user
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Device select item collection</returns>
   public async Task<IFilterableList<WidgetSelectItem>> GetUserWidgetSelectListAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var cacheKey = staticCacheManager.PrepareKeyForShortTermCache(CacheDefaults<WidgetSelectItem>.ByDynamicFilterCacheKey, "own", filter);
      return await staticCacheManager.GetAsync(cacheKey, acquire);

      async Task<IFilterableList<WidgetSelectItem>> acquire()
      {
         var filterProto = Auto.Mapper.Map<FilterProto>(filter);
         var query = await grpcClient.GetOwnWidgetSelectItemsAsync(filterProto);
         var widgets = Auto.Mapper.Map<FilterableList<WidgetSelectItem>>(query.Widgets);
         widgets.TotalCount = query.TotalCount ?? 0;

         return widgets;
      }
   }

   #endregion

   #region Update

   /// <summary>
   /// Updates a widget entity from the widget model
   /// </summary>
   /// <param name="model">Widget model</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task UpdateAsync(WidgetModel model)
   {
      ArgumentNullException.ThrowIfNull(model);
      ArgumentOutOfRangeException.ThrowIfLessThan(model.Id, 1);

      var proto = Auto.Mapper.Map<WidgetProto>(model);
      var reply = await grpcClient.UpdateAsync(proto);

      var widget = Auto.Mapper.Map<Widget>(reply);
      Auto.Mapper.Map(widget, model);

      var cacheKey = staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Widget>.ByIdCacheKey, model.Id);
      if (memoryCache.TryGetValue(cacheKey.Key, out object value) && value is not null && value is Widget savedWidget)
         Auto.Mapper.Map(model, savedWidget);

      await staticCacheManager.RemoveByPrefixAsync(CacheDefaults<PresentationSelectItem>.Prefix);
      await staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Presentation>.Prefix);
      await staticCacheManager.RemoveByPrefixAsync(CacheDefaults<MonitorView>.Prefix);
   }

   #endregion

   #region Insert

   /// <summary>
   /// Adds a widget entity by the widget model
   /// </summary>
   /// <param name="model">Widget model</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task InsertAsync(WidgetModel model)
   {
      ArgumentNullException.ThrowIfNull(model);
      ArgumentOutOfRangeException.ThrowIfNotEqual(model.Id, 0);

      // insert in data base
      var proto = Auto.Mapper.Map<WidgetProto>(model);
      var reply = await grpcClient.InsertAsync(proto);

      // update models
      var widget = Auto.Mapper.Map<Widget>(reply);
      Auto.Mapper.Map(widget, model);

      await staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Widget>.ByDynamicFilterPrefix, "own");
      await staticCacheManager.RemoveByPrefixAsync(CacheDefaults<Widget>.ByDynamicFilterPrefix, "all");
      var cacheKey = staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<Widget>.ByIdCacheKey, widget.Id);
      await staticCacheManager.SetAsync(cacheKey, widget);
   }

   #endregion

   #region Delete

   /// <summary>
   /// Deletes a widget entity by the widget model
   /// </summary>
   /// <param name="model">Widget model</param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   /// <exception cref="ArgumentOutOfRangeException"></exception>
   public async Task DeleteAsync(WidgetModel model)
   {
      ArgumentNullException.ThrowIfNull(model);
      ArgumentOutOfRangeException.ThrowIfLessThan(model.Id, 1);

      await grpcClient.DeleteAsync(new() { Id = model.Id });

      await clearCacheService.WidgetClearCache();
   }

   #endregion

   #endregion
}
