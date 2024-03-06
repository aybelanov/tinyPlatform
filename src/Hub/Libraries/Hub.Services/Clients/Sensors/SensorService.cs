using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Users;
using Hub.Data;
using Hub.Services.Clients.Devices;
using Hub.Services.Clients.Monitors;
using Hub.Services.Devices;
using Hub.Services.Localization;
using Microsoft.EntityFrameworkCore;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Sensors;

#pragma warning disable CS1591

public class SensorService(IRepository<Device> deviceRepository,
   IRepository<Sensor> sensorRepository,
   IRepository<Monitor> monitorRepository,
   IRepository<UserMonitor> userMonitorRepository,
   IStaticCacheManager staticCacheManager,
   IRepository<Presentation> monitorSensorWidgetRepository,
   IRepository<User> userRepository,
   IRepository<UserDevice> userDeviceRepository,
   IRepository<SensorWidget> sensorWidgetRepository,
   IRepository<LocalizedProperty> localizedRepository,
   IDeviceService deviceService,
   IMonitorService monitorService,
   IWorkContext workContext,
   ILocalizer localizer)
   : HubSensorService(deviceRepository,
      sensorRepository,
      sensorWidgetRepository,
      monitorSensorWidgetRepository,
      staticCacheManager,
      userRepository), ISensorService
{
   #region Fields

   private readonly IRepository<Device> _deviceRepository = deviceRepository;
   private readonly IRepository<Sensor> _sensorRepository = sensorRepository;
   private readonly IRepository<Monitor> _monitorRepository = monitorRepository;
   private readonly IRepository<SensorWidget> _sensorWidgetRepository = sensorWidgetRepository;
   private readonly IRepository<UserDevice> _userDeviceRepository = userDeviceRepository;
   private readonly IRepository<UserMonitor> _userMonitorRepository = userMonitorRepository;
   private readonly IRepository<Presentation> _monitorSensorWidgetRepository = monitorSensorWidgetRepository;
   private readonly IRepository<User> _userRepository = userRepository;
   private readonly IStaticCacheManager _staticCacheManager = staticCacheManager;
   private readonly IRepository<LocalizedProperty> _localizedRepository = localizedRepository;
   private readonly IDeviceService _deviceService = deviceService;
   private readonly IMonitorService _monitorService = monitorService;
   private readonly IWorkContext _workContext = workContext;
   private readonly ILocalizer _localizer = localizer;

   #endregion
   #region Ctor

   #endregion

   #region Utils

   /// <summary>
   /// Required for client query
   /// </summary>
   /// <param name="sensorQuery">Incomming sensor query</param>
   /// <returns>Result query</returns>
   private async Task<IQueryable<Sensor>> ProjectionQuery(IQueryable<Sensor> sensorQuery)
   {
      var lang = await _workContext.GetWorkingLanguageAsync();
      var deviceQuery = _deviceRepository.Table.AsNoTracking();

      var localeQuery =
      from lp in _localizedRepository.Table.AsNoTracking()
      where lp.LocaleKeyGroup == nameof(Sensor) && lp.LanguageId == lang.Id
      select new { lp.EntityId, lp.LocaleKey, lp.LocaleValue };

      sensorQuery =
         from s in sensorQuery
         join d in deviceQuery on s.DeviceId equals d.Id
         select new Sensor()
         {
            Id = s.Id,
            SystemName = s.SystemName,
            Configuration = s.Configuration,
            CreatedOnUtc = s.CreatedOnUtc,
            DeviceId = s.DeviceId,
            Enabled = s.Enabled,
            SensorType = s.SensorType,
            PriorityType = s.PriorityType,
            PictureId = s.PictureId,
            ShowInCommonLog = s.ShowInCommonLog,
            UpdatedOnUtc = s.UpdatedOnUtc,
            DeviceSystemName = d.SystemName,

            Name = localeQuery
            .Where(x => x.EntityId == s.Id && x.LocaleKey == nameof(Sensor.Name))
            .Select(x => x.LocaleValue)
            .FirstOrDefault(),

            Description = localeQuery
            .Where(x => x.EntityId == s.Id && x.LocaleKey == nameof(Sensor.Description))
            .Select(x => x.LocaleValue)
            .FirstOrDefault(),

            MeasureUnit = localeQuery
            .Where(x => x.EntityId == s.Id && x.LocaleKey == nameof(Sensor.MeasureUnit))
            .Select(x => x.LocaleValue)
            .FirstOrDefault(),
         };

      return sensorQuery;
   }



   #endregion

   #region Delete

   /// <inheritdoc/>
   public override async Task DeleteAsync(Sensor sensors)
   {
      ArgumentNullException.ThrowIfNull(sensors);

      // it's soft deleting entity. So we don't delete entity locales
      //await _localizer.DeleteLocaleAsync<Sensor>(sensors.Select(x => x.Id));

      await base.DeleteAsync(sensors);
   }

   #endregion

   #region Get

   /// <summary>
   /// User scope for sensors
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <returns>User scope query</returns>
   public IQueryable<Sensor> UserScope(long? userId)
   {
      var query = _sensorRepository.Table.AsNoTracking();

      if (userId != null)
      {
         query =
         (from s in query
          join d in _deviceService.UserScope(userId) on s.DeviceId equals d.Id
          select s)
         .Union
         (from s in query
          join sw in _sensorWidgetRepository.Table.AsNoTracking() on s.Id equals sw.SensorId
          join msw in _monitorSensorWidgetRepository.Table.AsNoTracking() on sw.Id equals msw.SensorWidgetId
          join m in _monitorService.UserScope(userId) on msw.MonitorId equals m.Id
          select s);
      }

      return query;
   }

   /// <summary>
   /// Gets sensors by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Filterable collection</returns>
   public async Task<IFilterableList<Sensor>> GetSensorsAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var sensorQuery = _sensorRepository.Table.AsNoTracking();

      sensorQuery = UserScope(filter.UserId);

      if (filter.DeviceId.HasValue)
         sensorQuery = sensorQuery.Where(x => x.DeviceId == filter.DeviceId);

      if (filter.DeviceIds?.Any() == true)
         sensorQuery = sensorQuery.Where(x => filter.DeviceIds.Contains(x.DeviceId));

      if (filter.SensorId.HasValue)
         sensorQuery = sensorQuery.Where(x => x.Id == filter.SensorId);

      if (filter.SensorIds?.Any() == true)
         sensorQuery = sensorQuery.Where(x => filter.SensorIds.Contains(x.Id));

      if (filter.WidgetId.HasValue)
         sensorQuery =
         from s in sensorQuery
         join sw in _sensorWidgetRepository.Table.AsNoTracking() on s.Id equals sw.SensorId
         where sw.WidgetId == filter.WidgetId
         select s;

      if (filter.WidgetIds?.Any() == true)
         sensorQuery =
         from s in sensorQuery
         join sw in _sensorWidgetRepository.Table.AsNoTracking() on s.Id equals sw.SensorId
         where filter.WidgetIds.Contains(sw.WidgetId)
         select s;

      if (filter.MonitorId.HasValue)
         sensorQuery =
         from s in sensorQuery
         join sw in _sensorWidgetRepository.Table.AsNoTracking() on s.Id equals sw.SensorId
         join msw in _monitorSensorWidgetRepository.Table.AsNoTracking() on sw.Id equals msw.SensorWidgetId
         where msw.MonitorId == filter.MonitorId.Value
         select s;

      if (filter.MonitorIds?.Any() == true)
         sensorQuery =
         from s in sensorQuery
         join sw in _sensorWidgetRepository.Table.AsNoTracking() on s.Id equals sw.SensorId
         join msw in _monitorSensorWidgetRepository.Table.AsNoTracking() on sw.Id equals msw.SensorWidgetId
         where filter.WidgetIds.Contains(sw.WidgetId)
         select s;

      sensorQuery = sensorQuery.Distinct();
      sensorQuery = await ProjectionQuery(sensorQuery);//, deviceQuery);
      sensorQuery = sensorQuery.ApplyClientQuery(filter);

      var result = await sensorQuery.FilterAsync(filter);
      return result;
   }

   /// <summary>
   /// Gets sensors for common log
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <returns>Sensor collection</returns>
   public async Task<IList<long>> GetCommonLogSensorIdsAsync(long userId)
   {
      var query =
         from s in _sensorRepository.Table.AsNoTracking()
         join d in _deviceRepository.Table.AsNoTracking() on s.DeviceId equals d.Id
         where d.OwnerId == userId && s.ShowInCommonLog
         select s.Id;

      return await query.ToListAsync();
   }


   /// <summary>
   /// Gets sensor select items by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Filterable sensor select item list connection</returns>
   public async Task<IFilterableList<SensorSelectItem>> GetSensorSelectItemListAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);
      ArgumentNullException.ThrowIfNull(filter.DeviceId);

      var lang = await _workContext.GetWorkingLanguageAsync();

      var deviceQuery = _deviceRepository.Table.AsNoTracking();

      if (filter.UserId != null)
      {
         deviceQuery =
            from d in deviceQuery
            join du in _userDeviceRepository.Table.AsNoTracking() on d.Id equals du.DeviceId into gr
            from i in gr.DefaultIfEmpty()
            where i != default && i.UserId == filter.UserId || d.OwnerId == filter.UserId
            select d;
      }

      var deviceIdsQuery = deviceQuery.Where(x => x.Id == filter.DeviceId).Select(x => x.Id);

      var sensorQuery = _sensorRepository.Table.AsNoTracking();
      sensorQuery = sensorQuery.ApplyClientQuery(filter);

      var query =
         from s in sensorQuery
         where deviceIdsQuery.Contains(s.DeviceId)
         select new SensorSelectItem()
         {
            Id = s.Id,
            SystemName = s.SystemName,
            Name =
             (from lp in _localizedRepository.Table.AsNoTracking()
              where lp.EntityId == s.Id
                 && lp.LanguageId == lang.Id
                 && lp.LocaleKeyGroup == nameof(Sensor)
                 && lp.LocaleKey == nameof(Sensor.Name)
              select lp.LocaleValue).FirstOrDefault(),

            MeasureUnit =
             (from lp in _localizedRepository.Table.AsNoTracking()
              where lp.EntityId == s.Id
                 && lp.LanguageId == lang.Id
                 && lp.LocaleKeyGroup == nameof(Sensor)
                 && lp.LocaleKey == nameof(Sensor.MeasureUnit)
              select lp.LocaleValue).FirstOrDefault()
         };

      var result = await query.FilterAsync(filter);
      return result;
   }

   #endregion

   #region Update

   /// <summary>
   /// Update a sensor entity
   /// </summary>
   /// <param name="sensor">Sensor entity</param>
   /// <returns></returns>
   public override async Task UpdateAsync(Sensor sensor)
   {
      ArgumentNullException.ThrowIfNull(sensor);

      await base.UpdateAsync(sensor);

      // update/insert locales
      await _localizer.SaveLocaleAsync(sensor, await _workContext.GetWorkingLanguageAsync());
   }

   #endregion

   #region Insert

   /// <summary>
   /// Inserts sensor to database
   /// </summary>
   /// <param name="sensor">Adding device entity</param>
   /// <returns>Device identifier</returns>
   public override async Task<long> InsertAsync(Sensor sensor)
   {
      ArgumentNullException.ThrowIfNull(sensor);

      await base.InsertAsync(sensor);

      // update/insert locales
      await _localizer.SaveLocaleAsync(sensor, await _workContext.GetWorkingLanguageAsync());

      return sensor.Id;
   }

   #endregion

   #region Common

   /// <summary>
   /// Is a sensor in a user scope
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="sensorId">Sensor identifier</param>
   /// <returns>Result: true - in scope; false - not in scope.</returns>
   public async Task<bool> IsInUserScopeAsync(long userId, long sensorId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(userId, 1);
      ArgumentOutOfRangeException.ThrowIfLessThan(sensorId, 1);

      var query =
         from s in _sensorRepository.Table.AsNoTracking().Where(x => x.Id == sensorId)
         join d in _deviceRepository.Table.AsNoTracking().Where(x => x.OwnerId == userId)
         on s.DeviceId equals d.Id
         select s;

      return await query.AnyAsync();
   }

   #endregion

}
#pragma warning restore CS1591