using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Users;
using Hub.Data;
using Hub.Services.Localization;
using Microsoft.EntityFrameworkCore;
using Shared.Clients;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Widgets;

/// <summary>
/// Widget service implementation
/// </summary>
/// <remarks>
/// IoC Ctor
/// </remarks>
public class WidgetService(IRepository<Widget> widgetRepository,
    IWorkContext workContext,
    IRepository<Sensor> sensorRepository,
    IRepository<Device> deviceRepository,
    IRepository<Presentation> monitorSensorWidgetRepository,
    IRepository<User> userRepository,
    IRepository<SensorWidget> sensorWidgetRepository,
    IRepository<LocalizedProperty> localizedRepository,
    ILocalizer localizer,
    IStaticCacheManager staticCachemanager,
    UserSettings userSettings) : IWidgetService
{
   #region Fields

   private readonly IRepository<Widget> _widgetRepository = widgetRepository;
   private readonly IRepository<SensorWidget> _sensorWidgetRepository = sensorWidgetRepository;
   private readonly IRepository<Sensor> _sensorRepository = sensorRepository;
   private readonly IRepository<Device> _deviceRepository = deviceRepository;
   private readonly IRepository<Presentation> _presentationRepository = monitorSensorWidgetRepository;
   private readonly IRepository<User> _userRepository = userRepository;
   private readonly IWorkContext _workContext = workContext;
   private readonly IRepository<LocalizedProperty> _localizedRepository = localizedRepository;
   private readonly ILocalizer _localizer = localizer;
   private readonly UserSettings _userSettings = userSettings;
   private readonly IStaticCacheManager _staticCacheManager = staticCachemanager;

   #endregion

   #region Utilities

   //private async Task<IQueryable<Widget>> ProjectionQuery(DynamicFilter filter, IQueryable<Widget> query)
   //{
   //   // user query
   //   var userQuery = _userRepository.Table.AsNoTracking();

   //   // filter user by owner name column
   //   if (filter.AdditionalQueries.TryGetValue(nameof(User), out var additionalUserQuery) && !string.IsNullOrEmpty(additionalUserQuery))
   //   {
   //      additionalUserQuery = additionalUserQuery.Replace("OwnerName", _userSettings.UsernamesEnabled ? nameof(User.Username) : nameof(User.Email));
   //      userQuery = userQuery.Where(additionalUserQuery);
   //   }

   //   var lang = await _workContext.GetWorkingLanguageAsync();

   //   var localeQuery =
   //   from lp in _localizedRepository.Table.AsNoTracking()
   //   where lp.LocaleKeyGroup == nameof(Widget) && lp.LanguageId == lang.Id
   //   select new { lp.EntityId, lp.LocaleKey, lp.LocaleValue };

   //   query =
   //      from w in query
   //      join u in userQuery on w.UserId equals u.Id
   //      select new Widget()
   //      {
   //         Id = w.Id,
   //         UserId = w.UserId,
   //         CreatedOnUtc = w.CreatedOnUtc,
   //         DisplayOrder = w.DisplayOrder,
   //         HistoryPointsCount = w.HistoryPointsCount,
   //         MaxCriticalValue = w.MaxCriticalValue,
   //         MaxValue = w.MaxValue,
   //         MinCriticalValue = w.MinCriticalValue,
   //         MinValue = w.MinValue,
   //         PictureId = w.PictureId,
   //         Scheme = w.Scheme,
   //         ShowAsAreachart = w.ShowAsAreachart,
   //         ShowCriticalValueNotification = w.ShowCriticalValueNotification,
   //         ShowHistoryAnnotations = w.ShowHistoryAnnotations,
   //         ShowHistory = w.ShowHistory,
   //         ShowHistoryTrends = w.ShowHistoryTrends,
   //         ShowNotificationForAll = w.ShowNotificationForAll,
   //         SmothHistoryChart = w.SmothHistoryChart,
   //         UpdatedOnUtc = w.UpdatedOnUtc,
   //         UseValueConstraint = w.UseValueConstraint,
   //         WidgetType = w.WidgetType,

   //         OwnerName = _userSettings.UsernamesEnabled ? u.Username : u.Email,

   //         Name = localeQuery
   //            .Where(x => x.EntityId == w.Id && x.LocaleKey == nameof(Widget.Name))
   //            .Select(x => x.LocaleValue)
   //            .FirstOrDefault(),

   //         Description = localeQuery
   //            .Where(x => x.EntityId == w.Id && x.LocaleKey == nameof(Widget.Description))
   //            .Select(x => x.LocaleValue)
   //            .FirstOrDefault(),
   //      };

   //   return query;
   //}

   #endregion

   #region Delete

   /// <summary>
   /// Deletes widget from the datavase
   /// </summary>
   /// <param name="widget">Widget entity</param>
   /// <returns></returns>
   public async Task DeleteAsync(Widget widget)
   {
      ArgumentNullException.ThrowIfNull(widget);

      // it's soft deleting entity. So we don't delete entity locales
      //await _localizer.DeleteLocaleAsync<Device>(devices.Select(x => x.Id));

      var sensorWidgets = _sensorWidgetRepository.Table.AsNoTracking().Where(x => x.WidgetId == widget.Id);

      var presentations =
         from p in _presentationRepository.Table.AsNoTracking()
         join sw in sensorWidgets on p.SensorWidgetId equals sw.Id
         select p;

      await presentations.ExecuteDeleteAsync();
      await sensorWidgets.ExecuteDeleteAsync();

      await _widgetRepository.DeleteAsync(widget);
   }

   #endregion

   #region Get

   /// <summary>
   /// Gets a widget by its identifier
   /// </summary>
   /// <param name="widgetId">Widget identifier</param>
   /// <returns>Widget entity</returns>
   public Task<Widget> GetByIdAsync(long widgetId)
   {
      var widget = _widgetRepository.GetByIdAsync(widgetId, default);
      return widget;
   }

   /// <summary>
   /// Gets widgets bby the dynamic filter
   /// </summary>
   /// <param name="filter">Dinamic filter</param>
   /// <returns>Widget filterable collection</returns>
   public async Task<IFilterableList<Widget>> GetAllWidgetsAsync(DynamicFilter filter)
   {
      var query = _widgetRepository.Table.AsNoTracking();

      if (filter.UserId.HasValue)
         query = query.Where(x => x.UserId == filter.UserId);

      if (filter.UserIds?.Any() == true)
         query = query.Where(x => filter.UserIds.Contains(x.UserId));

      if (filter.SensorId.HasValue)
         query = from w in query
                 join sw in _sensorWidgetRepository.Table.AsNoTracking() on w.Id equals sw.WidgetId
                 where sw.SensorId == filter.SensorId
                 select w;

      if (filter.SensorIds?.Any() == true)
         query = from w in query
                 join sw in _sensorWidgetRepository.Table.AsNoTracking() on w.Id equals sw.WidgetId
                 where filter.SensorIds.Contains(sw.SensorId)
                 select w;

      if (filter.MonitorId.HasValue)
         query = from w in query
                 join msw in _presentationRepository.Table.AsNoTracking() on w.Id equals msw.WidgetId
                 where msw.MonitorId == filter.MonitorId
                 select w;

      if (filter.MonitorIds?.Any() == true)
         query = from w in query
                 join msw in _presentationRepository.Table.AsNoTracking() on w.Id equals msw.WidgetId
                 where filter.MonitorIds.Contains(msw.MonitorId)
                 select w;

      // user query
      var userQuery = _userRepository.Table.AsNoTracking();

      // filter user by owner name column
      if (filter.AdditionalQueries.TryGetValue(nameof(User), out var additionalUserQuery) && !string.IsNullOrEmpty(additionalUserQuery))
      {
         additionalUserQuery = additionalUserQuery.Replace("OwnerName", _userSettings.UsernamesEnabled ? nameof(User.Username) : nameof(User.Email));
         userQuery = userQuery.Where(additionalUserQuery);
      }

      var lang = await _workContext.GetWorkingLanguageAsync();
      var localeQuery = _localizedRepository.Table.AsNoTracking().Where(lp => lp.LocaleKeyGroup == nameof(Widget) && lp.LanguageId == lang.Id);

      query =
         from w in query
         join u in userQuery on w.UserId equals u.Id
         select new Widget()
         {
            Id = w.Id,
            UserId = w.UserId,
            CreatedOnUtc = w.CreatedOnUtc,
            DisplayOrder = w.DisplayOrder,
            PictureId = w.PictureId,
            LiveSchemePictureId = w.LiveSchemePictureId,
            Adjustment = w.Adjustment,
            WidgetType = w.WidgetType,

            OwnerName = _userSettings.UsernamesEnabled ? u.Username : u.Email,

            Name = localeQuery
               .Where(x => x.EntityId == w.Id && x.LocaleKey == nameof(Widget.Name))
               .Select(x => x.LocaleValue)
               .FirstOrDefault(),

            Description = localeQuery
               .Where(x => x.EntityId == w.Id && x.LocaleKey == nameof(Widget.Description))
               .Select(x => x.LocaleValue)
               .FirstOrDefault(),
         };

      query = query.ApplyClientQuery(filter);

      var result = await query.FilterAsync(filter);
      return result;
   }

   /// <summary>
   /// Gets all widget select list by the dynamic filter
   /// </summary>
   /// <param name="filter">Dinamic filter</param>
   /// <returns>Widget filterable collection</returns>
   public async Task<IFilterableList<WidgetSelectItem>> GetAllWidgetSelectListAsync(DynamicFilter filter)
   {
      var widgetQuery = _widgetRepository.Table.AsNoTracking();
      widgetQuery = widgetQuery.ApplyClientQuery(filter);

      var lang = await _workContext.GetWorkingLanguageAsync();
      var localeQuery = _localizedRepository.Table.AsNoTracking().Where(lp => lp.LocaleKeyGroup == nameof(Widget) && lp.LanguageId == lang.Id);

      var query =
      from w in widgetQuery
      join u in _userRepository.Table.AsNoTracking() on w.UserId equals u.Id
      select new WidgetSelectItem()
      {
         Id = w.Id,
         OwnerName = _userSettings.UsernamesEnabled ? u.Username : u.Email,
         Name = localeQuery
          .Where(x => x.EntityId == w.Id && x.LocaleKey == nameof(Widget.Name))
          .Select(x => x.LocaleValue)
          .FirstOrDefault()
      };

      var result = await query.FilterAsync(filter);
      return result;
   }

   /// <summary>
   /// Gets own widget select list by the dynamic filter
   /// </summary>
   /// <param name="filter">Dinamic filter</param>
   /// <returns>Widget filterable collection</returns>
   public async Task<IFilterableList<WidgetSelectItem>> GetOwnWidgetSelectListAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);
      ArgumentNullException.ThrowIfNull(filter.UserId);

      var widgetQuery = _widgetRepository.Table.AsNoTracking().Where(x => x.UserId == filter.UserId);
      widgetQuery = widgetQuery.ApplyClientQuery(filter);

      var lang = await _workContext.GetWorkingLanguageAsync();
      var localeQuery = _localizedRepository.Table.AsNoTracking().Where(lp => lp.LocaleKeyGroup == nameof(Widget) && lp.LanguageId == lang.Id);

      var query =
      from w in widgetQuery
      select new WidgetSelectItem()
      {
         Id = w.Id,
         Name = localeQuery
          .Where(x => x.EntityId == w.Id && x.LocaleKey == nameof(Widget.Name))
          .Select(x => x.LocaleValue)
          .FirstOrDefault()
      };

      var result = await query.FilterAsync(filter);
      return result;
   }

   #endregion

   #region Insert

   /// <summary>
   /// Adds a widget to the database
   /// </summary>
   /// <param name="widget">Widget entity</param>
   /// <returns>Widget identifier</returns>
   public async Task<long> InsertAsync(Widget widget)
   {
      ArgumentNullException.ThrowIfNull(widget);

      await _widgetRepository.InsertAsync(widget);

      // update/insert locales
      await _localizer.SaveLocaleAsync(widget, await _workContext.GetWorkingLanguageAsync());

      return widget.Id;
   }

   #endregion

   #region Update

   /// <summary>
   /// Update a widget
   /// </summary>
   /// <param name="widget">Widget entity</param>
   /// <param name="updateLocale">Update locales</param>
   /// <returns></returns>
   public async Task UpdateAsync(Widget widget, bool updateLocale = true)
   {
      ArgumentNullException.ThrowIfNull(widget);

      await _widgetRepository.UpdateAsync(widget);

      // update/insert locales
      if (updateLocale)
         await _localizer.SaveLocaleAsync(widget, await _workContext.GetWorkingLanguageAsync());
   }

   #endregion

   #region Mapping

   /// <summary>
   /// Is a widget in a user scope
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="widgetId">Widget identifier</param>
   /// <returns>Result: true - in scope; false - not in scope.</returns>
   public async Task<bool> IsInUserScopeAsync(long userId, long widgetId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(userId, 1);
      ArgumentOutOfRangeException.ThrowIfLessThan(widgetId, 1);

      var query = from w in _widgetRepository.Table.AsNoTracking()
                  where w.UserId == userId && w.Id == widgetId
                  select w;

      return await query.AnyAsync();
   }

   #endregion
}
