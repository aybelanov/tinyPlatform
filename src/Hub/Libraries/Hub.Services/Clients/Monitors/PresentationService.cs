using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Localization;
using Hub.Data;
using Hub.Services.Localization;
using Microsoft.EntityFrameworkCore;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Monitors;

/// <summary>
/// Presentation service implementation
/// </summary>
/// <remarks>
/// IoC Ctor
/// </remarks>
public class PresentationService(IRepository<LocalizedProperty> localizedRepository,
   IRepository<Sensor> sensorRepository,
   IWorkContext workContext,
   IRepository<Widget> widgetRepository,
   IRepository<SensorWidget> sensorWidgetRepository,
   IRepository<Device> deviceRepository,
   IRepository<Presentation> presentationRepository,
   IRepository<Monitor> monitorRepository,
   ILocalizer localizer) : IPresentationService
{
   
   #region Methods

   /// <summary>
   /// Gets all sensor-to-widget mapping select list by the dynamic filter
   /// </summary>
   /// <param name="filter">Dinamic filter</param>
   /// <returns>Widget filterable collection</returns>
   public async Task<IFilterableList<PresentationSelectItem>> GetAllPresentationSelectListAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var sensorQuery = sensorRepository.Table.AsNoTracking();

      //var widgetQuery = _widgetRepository.Table.AsNoTracking().Where(x => x.UserId == filter.UserId);

      var lang = await workContext.GetWorkingLanguageAsync();
      var widgetLocaleQuery = localizedRepository.Table.AsNoTracking().Where(lp => lp.LocaleKeyGroup == nameof(Widget) && lp.LanguageId == lang.Id);
      var sensorLocaleQuery = localizedRepository.Table.AsNoTracking().Where(lp => lp.LocaleKeyGroup == nameof(Sensor) && lp.LanguageId == lang.Id);
      var deviceLocaleQuery = localizedRepository.Table.AsNoTracking().Where(lp => lp.LocaleKeyGroup == nameof(Device) && lp.LanguageId == lang.Id);

      var sensorWidgetQuery = sensorWidgetRepository.Table.AsNoTracking();

      if (filter.SensorWidgetId.HasValue)
         sensorWidgetQuery = sensorWidgetQuery.Where(x => x.Id == filter.SensorWidgetId.Value);

      if (filter.SensorWidgetIds?.Any() == true)
         sensorWidgetQuery = sensorWidgetQuery.Where(x => filter.SensorWidgetIds.Contains(x.Id));

      var query =
         from sw in sensorWidgetQuery
         join s in sensorQuery on sw.SensorId equals s.Id
         select new PresentationSelectItem()
         {
            Id = sw.Id,
            SensorId = sw.SensorId,
            WidgetId = sw.WidgetId,
            DeviceId = s.DeviceId,
            SensorName = sensorLocaleQuery
               .Where(x => x.LocaleKey == nameof(Sensor.Name) && x.EntityId == sw.SensorId)
               .Select(x => x.LocaleValue)
               .FirstOrDefault(),

            WidgetName = widgetLocaleQuery
               .Where(x => x.LocaleKey == nameof(Widget.Name) && x.EntityId == sw.WidgetId)
               .Select(x => x.LocaleValue)
               .FirstOrDefault(),

            DeviceName = deviceLocaleQuery
               .Where(x => x.LocaleKey == nameof(Device.Name) && x.EntityId == s.DeviceId)
               .Select(x => x.LocaleValue)
               .FirstOrDefault(),
         };

      query = query.ApplyClientQuery(filter);
      var result = await query.FilterAsync(filter);
      return result;
   }

   /// <summary>
   /// Gets own (user) sensor-to-widget mapping select list by the dynamic filter
   /// </summary>
   /// <param name="filter">Dinamic filter</param>
   /// <returns>Widget filterable collection</returns>
   public async Task<IFilterableList<PresentationSelectItem>> GetOwnPresentationSelectListAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);
      ArgumentNullException.ThrowIfNull(filter.UserId);

      var sensorQuery =
         from s in sensorRepository.Table.AsNoTracking()
         join d in deviceRepository.Table.AsNoTracking().Where(x => x.OwnerId == filter.UserId) on s.DeviceId equals d.Id
         select s;

      //var widgetQuery = _widgetRepository.Table.AsNoTracking().Where(x => x.UserId == filter.UserId);

      var lang = await workContext.GetWorkingLanguageAsync();
      var widgetLocaleQuery = localizedRepository.Table.AsNoTracking().Where(lp => lp.LocaleKeyGroup == nameof(Widget) && lp.LanguageId == lang.Id);
      var sensorLocaleQuery = localizedRepository.Table.AsNoTracking().Where(lp => lp.LocaleKeyGroup == nameof(Sensor) && lp.LanguageId == lang.Id);
      var deviceLocaleQuery = localizedRepository.Table.AsNoTracking().Where(lp => lp.LocaleKeyGroup == nameof(Device) && lp.LanguageId == lang.Id);

      var sensorWidgetQuery = sensorWidgetRepository.Table.AsNoTracking();

      if(filter.SensorWidgetId.HasValue)
         sensorWidgetQuery = sensorWidgetQuery.Where(x=>x.Id ==  filter.SensorWidgetId.Value);

      if (filter.SensorWidgetIds?.Any() == true)
         sensorWidgetQuery = sensorWidgetQuery.Where(x => filter.SensorWidgetIds.Contains(x.Id));

      var query =
         from sw in sensorWidgetQuery
         join s in sensorQuery on sw.SensorId equals s.Id
         select new PresentationSelectItem()
         {
            Id = sw.Id,
            SensorId = sw.SensorId,
            WidgetId = sw.WidgetId,
            DeviceId = s.DeviceId,
            SensorName = sensorLocaleQuery
               .Where(x => x.LocaleKey == nameof(Sensor.Name) && x.EntityId == sw.SensorId)
               .Select(x => x.LocaleValue)
               .FirstOrDefault(),

            WidgetName = widgetLocaleQuery
               .Where(x => x.LocaleKey == nameof(Widget.Name) && x.EntityId == sw.WidgetId)
               .Select(x => x.LocaleValue)
               .FirstOrDefault(),

            DeviceName = deviceLocaleQuery
               .Where(x => x.LocaleKey == nameof(Device.Name) && x.EntityId == s.DeviceId)
               .Select(x => x.LocaleValue)
               .FirstOrDefault(),
         };

      query = query.ApplyClientQuery(filter);
      var result = await query.FilterAsync(filter);
      return result;
   }

   /// <summary>
   /// Gets monitor presentation collection by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Filterable collection of the monitor prentations</returns>
   public async Task<IFilterableList<Presentation>> GetPresentationsAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter.MonitorId);
      ArgumentOutOfRangeException.ThrowIfLessThan(filter.MonitorId.Value, 1);

      var monitorQuery = monitorRepository.Table.AsNoTracking().Where(x => x.Id == filter.MonitorId.Value);

      if (filter.UserId.HasValue)
         monitorQuery = monitorQuery.Where(x => x.OwnerId == filter.UserId.Value);

      var lang = await workContext.GetWorkingLanguageAsync();
      var widgetLocaleQuery = localizedRepository.Table.AsNoTracking().Where(lp => lp.LocaleKeyGroup == nameof(Widget) && lp.LanguageId == lang.Id);
      var sensorLocaleQuery = localizedRepository.Table.AsNoTracking().Where(lp => lp.LocaleKeyGroup == nameof(Sensor) && lp.LanguageId == lang.Id);
      var deviceLocaleQuery = localizedRepository.Table.AsNoTracking().Where(lp => lp.LocaleKeyGroup == nameof(Device) && lp.LanguageId == lang.Id);
      var presentationLocaleQuery = localizedRepository.Table.AsNoTracking().Where(lp => lp.LocaleKeyGroup == nameof(Presentation) && lp.LanguageId == lang.Id);

      var query =
         from msw in presentationRepository.Table.AsNoTracking()
         join m in monitorQuery on msw.MonitorId equals m.Id
         join sw in sensorWidgetRepository.Table.AsNoTracking() on msw.SensorWidgetId equals sw.Id
         join s in sensorRepository.Table.AsNoTracking() on sw.SensorId equals s.Id
         select new Presentation()
         {
            Id = msw.Id,
            SensorWidgetId = msw.SensorWidgetId,
            MonitorId = msw.MonitorId,

            SensorId = sw.SensorId,
            WidgetId = sw.WidgetId,
            DeviceId = s.DeviceId,
            DisplayOrder = msw.DisplayOrder,

            DeviceName = deviceLocaleQuery
            .Where(x => x.LocaleKey == nameof(Device.Name) && x.EntityId == s.DeviceId)
            .Select(x => x.LocaleValue)
            .FirstOrDefault(),

            SensorName = sensorLocaleQuery
            .Where(x => x.LocaleKey == nameof(Sensor.Name) && x.EntityId == sw.SensorId)
            .Select(x => x.LocaleValue)
            .FirstOrDefault(),

            WidgetName = widgetLocaleQuery
            .Where(x => x.LocaleKey == nameof(Widget.Name) && x.EntityId == sw.WidgetId)
            .Select(x => x.LocaleValue)
            .FirstOrDefault(),

            Name = presentationLocaleQuery
            .Where(x => x.LocaleKey == nameof(Presentation.Name) && x.EntityId == msw.Id)
            .Select(x => x.LocaleValue)
            .FirstOrDefault(),

            Description = presentationLocaleQuery
            .Where(x => x.LocaleKey == nameof(Presentation.Description) && x.EntityId == msw.Id)
            .Select(x => x.LocaleValue)
            .FirstOrDefault()
         };

      query = query.Distinct();
      query = query.ApplyClientQuery(filter);
      var result = await query.FilterAsync(filter);
      return result;
   }

   /// <summary>
   /// Map a monitor to a sensor and to a sensor presentation
   /// </summary>
   /// <param name="map">Monitor presentation entity</param>
   /// <returns>Async task</returns>
   public virtual async Task MapPresentationAsync(Presentation map)
   {
      ArgumentNullException.ThrowIfNull(map);
      ArgumentOutOfRangeException.ThrowIfLessThan(map.MonitorId, 1);
      ArgumentOutOfRangeException.ThrowIfLessThan(map.SensorWidgetId, 1);

      var mapping = await presentationRepository.Table.FirstOrDefaultAsync(x => x.MonitorId == map.MonitorId && x.SensorWidgetId == map.SensorWidgetId);

      if (mapping != null)
         return;

      await presentationRepository.InsertAsync(map);

      // update/insert locales
      await localizer.SaveLocaleAsync(map, await workContext.GetWorkingLanguageAsync());
   }

   /// <summary>
   /// Update a monitor to a sensor-to-widget presentation
   /// </summary>
   /// <param name="map">Monitor presentation entity</param>
   /// <returns>Async task</returns>
   public async Task UpdateMapPresentationAsync(Presentation map)
   {
      ArgumentNullException.ThrowIfNull(map);
      ArgumentOutOfRangeException.ThrowIfLessThan(map.SensorWidgetId, 1);

      await presentationRepository.UpdateAsync(map);

      // update/insert locales
      await localizer.SaveLocaleAsync(map, await workContext.GetWorkingLanguageAsync());
   }


   /// <summary>
   /// Unmap a monitor to a sensor and to a sensor presentation
   /// </summary>
   /// <param name="map">Monitor presentation entity</param>
   /// <returns>Async task</returns>
   public virtual async Task UnmapPresentationAsync(Presentation map)
   {
      ArgumentNullException.ThrowIfNull(map);

      // it's not soft deleting entity. So we delete entity locales
      await localizer.DeleteLocaleAsync<Presentation>(map.Id);

      await presentationRepository.DeleteAsync(map);
   }

   /// <summary>
   /// Gets presentation entity by the identifier
   /// </summary>
   /// <param name="presentationId">Presentation identifier</param>
   /// <returns>Presentation entity</returns>
   public async Task<Presentation> GetPresentationByIdAsync(long presentationId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(presentationId, 1);

      var presentation = await presentationRepository.Table.AsNoTracking().FirstOrDefaultAsync(x => x.Id == presentationId);
      return presentation;
   }

   /// <summary>
   /// Maps a sensor to a widget
   /// </summary>
   /// <param name="sensorId">Sensor identifier</param>
   /// <param name="widgetId">Widget identifier</param>
   /// <returns></returns>
   public async Task MapSensorToWidgetAsync(long sensorId, long widgetId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(sensorId, 1);
      ArgumentOutOfRangeException.ThrowIfLessThan(widgetId, 1);

      var mapping = await sensorWidgetRepository.Table.FirstOrDefaultAsync(x => x.SensorId == sensorId && x.WidgetId == widgetId);

      if (mapping is not null)
         return;

      await sensorWidgetRepository.InsertAsync(new SensorWidget() { SensorId = sensorId, WidgetId = widgetId });
   }

   /// <summary>
   /// Unmaps a sensor to a widget
   /// </summary>
   /// <param name="sensorId">Sensor identifier</param>
   /// <param name="widgetId">Widget identifier</param>
   /// <returns></returns>
   public async Task UnmapSensorFromWidgetAsync(long sensorId, long widgetId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(sensorId, 1);
      ArgumentOutOfRangeException.ThrowIfLessThan(widgetId, 1);

      var mapping = await sensorWidgetRepository.Table.AsNoTracking().FirstOrDefaultAsync(x => x.SensorId == sensorId && x.WidgetId == widgetId);

      if (mapping is null)
         return;

      var monitorMappings = presentationRepository.Table.AsNoTracking().Where(x => x.SensorWidgetId == mapping.Id);
      await monitorMappings.ExecuteDeleteAsync();

      await sensorWidgetRepository.DeleteAsync(mapping);
   }

   /// <summary>
   /// Is a sensor-to-widget map in the user scope
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="sensorWidgetId">Sensor-to-widget identifier</param>
   /// <returns>Result: true - into the scope, false - not inot the scope</returns>
   public async Task<bool> IsSensorWidgetInUserScopeAsync(long userId, long sensorWidgetId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(userId, 1);
      ArgumentOutOfRangeException.ThrowIfLessThan(sensorWidgetId, 1);

      var query =
         from sw in sensorWidgetRepository.Table.AsNoTracking().Where(x => x.Id == sensorWidgetId)
         join w in widgetRepository.Table.AsNoTracking().Where(x => x.UserId == userId) on sw.WidgetId equals w.Id
         join s in sensorRepository.Table.AsNoTracking() on sw.SensorId equals s.Id
         join d in deviceRepository.Table.AsNoTracking().Where(x => x.OwnerId == userId) on s.DeviceId equals d.Id
         select s;

      return await query.AnyAsync();
   }

   #endregion
}
