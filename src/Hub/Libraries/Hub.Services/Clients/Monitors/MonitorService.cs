using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Users;
using Hub.Data;
using Hub.Services.Localization;
using Microsoft.EntityFrameworkCore;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Monitors;


/// <summary>
/// Servive of monitors
/// </summary>
public class MonitorService(IRepository<Device> deviceRepository,
   IRepository<Monitor> monitorRepository,
   IRepository<Sensor> sensorRepository,
   IRepository<Widget> widgetRepository,
   IRepository<Presentation> presentationRepository,
   IRepository<SensorWidget> sensorWidgetRepository,
   IRepository<User> userRepository,
   IRepository<UserMonitor> monitorUserRepository,
   IWorkContext workContext,
   DeviceSettings deviceSettings,
   ICommunicator communicator,
   IRepository<LocalizedProperty> localizedRepository,
   UserSettings userSettings,
   ILocalizer localizer) : IMonitorService
{

   #region Projection query

   /// <summary>
   /// Required for client query
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <param name="query">Income query</param>
   /// <returns>Result query</returns>
   private async Task<IQueryable<Monitor>> ProjectionQuery(DynamicFilter filter, IQueryable<Monitor> query)
   {
      // user query
      var userQuery = userRepository.Table.AsNoTracking();

      // filter user by owner name column
      if (filter.AdditionalQueries.TryGetValue(nameof(User), out var additionalUserQuery) && !string.IsNullOrEmpty(additionalUserQuery))
      {
         additionalUserQuery = additionalUserQuery.Replace("OwnerName", userSettings.UsernamesEnabled ? nameof(User.Username) : nameof(User.Email));
         userQuery = userQuery.Where(additionalUserQuery);
      }

      var lang = await workContext.GetWorkingLanguageAsync();

      var localeQuery =
      from lp in localizedRepository.Table.AsNoTracking()
      where lp.LocaleKeyGroup == nameof(Monitor) && lp.LanguageId == lang.Id
      select lp;

      // with locale dictionaries
      // we include this here to make one query
      query =
      from m in query
      join u in userQuery on m.OwnerId equals u.Id
      select new Monitor()
      {
         Id = m.Id,
         ShowInMenu = m.ShowInMenu,
         DisplayOrder = m.DisplayOrder,
         PictureId = m.PictureId,
         OwnerId = m.OwnerId,
         OwnerName = userSettings.UsernamesEnabled ? u.Username : u.Email,

         MenuItem = localeQuery
         .Where(x => x.EntityId == m.Id && x.LocaleKey == nameof(Monitor.MenuItem))
         .Select(x => x.LocaleValue)
         .FirstOrDefault(),

         Name = localeQuery
         .Where(x => x.EntityId == m.Id && x.LocaleKey == nameof(Monitor.Name))
         .Select(x => x.LocaleValue)
         .FirstOrDefault(),

         Description = localeQuery
         .Where(x => x.EntityId == m.Id && x.LocaleKey == nameof(Monitor.Description))
         .Select(x => x.LocaleValue)
         .FirstOrDefault(),
      };

      return query;
   }

   /// <summary>
   /// User scope for monitors
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="viaJoin">Query scope via join (for non-database type projection)</param>
   /// <returns>User scope query</returns>
   public IQueryable<Monitor> UserScope(long? userId, bool viaJoin = false)
   {
      var query = monitorRepository.Table.AsNoTracking();

      if (userId.HasValue)
      {
         if (viaJoin)
         {
            query =
            from m in query
            join mu in monitorUserRepository.Table.AsNoTracking() on m.Id equals mu.MonitorId into mgroup
            from g in mgroup.DefaultIfEmpty()
            where g != default && g.UserId == userId || m.OwnerId == userId
            select m;
         }
         else
         {
            query =
            (from m in query
             where m.OwnerId == userId
             select m)
             .Union
            (from m in query
             join um in monitorUserRepository.Table.AsNoTracking() on m.Id equals um.MonitorId
             where um.UserId == userId
             select m).AsSplitQuery();
         }
      }

      return query;
   }

   #endregion

   #region Methods

   #region Delete

   /// <summary>
   /// Delete monitor entities
   /// </summary>
   /// <param name="monitor">Deleting monitor entity</param>
   /// <returns>Async operation</returns>
   public virtual async Task DeleteAsync(Monitor monitor)
   {
      ArgumentNullException.ThrowIfNull(monitor);

      var presentations = presentationRepository.Table.Where(x => x.MonitorId == monitor.Id);
      await presentations.ExecuteDeleteAsync();

      // it's soft deleting entity. So we don't delete entity locales
      //await _localizer.DeleteLocaleAsync<Monitor>(monitors.Select(x => x.Id));
      await monitorRepository.DeleteAsync(monitor);
   }


   #endregion

   #region Get

   /// <summary>
   /// Gets a monitor view
   /// </summary>
   /// <param name="monitorId">Monitor identifier</param>
   /// <param name="userId">User identifier</param>
   /// <returns>Monitor view</returns>
   public async Task<Monitor> GetMonitorViewAsync(long monitorId, long? userId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(monitorId, 1);

      var onlineDevices = await communicator.GetOnlineDeviceIdsAsync();
      var beenRecenlyLimit = DateTime.UtcNow.AddMinutes(-deviceSettings.BeenRecentlyMinutes);
      var lang = await workContext.GetWorkingLanguageAsync();

      var monitorLocaleQuery =
      from lp in localizedRepository.Table.AsNoTracking()
      where lp.LocaleKeyGroup == nameof(Monitor) && lp.LanguageId == lang.Id
      select lp;

      var deviceLocaleQuery =
      from lp in localizedRepository.Table.AsNoTracking()
      where lp.LocaleKeyGroup == nameof(Device) && lp.LanguageId == lang.Id
      select lp;

      var sensorLocaleQuery =
      from lp in localizedRepository.Table.AsNoTracking()
      where lp.LocaleKeyGroup == nameof(Sensor) && lp.LanguageId == lang.Id
      select lp;

      var widgetLocaleQuery =
      from lp in localizedRepository.Table.AsNoTracking()
      where lp.LocaleKeyGroup == nameof(Widget) && lp.LanguageId == lang.Id
      select lp;

      var presentationLocaleQuery =
      from lp in localizedRepository.Table.AsNoTracking()
      where lp.LocaleKeyGroup == nameof(Presentation) && lp.LanguageId == lang.Id
      select lp;

      var monitorQuery = UserScope(userId, true);

      var query =
      from m in monitorQuery
      join u in userRepository.Table.AsNoTracking() on m.OwnerId equals u.Id
      where m.Id == monitorId
      select new Monitor
      {
         Id = m.Id,
         OwnerId = u.Id,
         OwnerName = userSettings.UsernamesEnabled ? u.Username : u.Email,
         
         Name = monitorLocaleQuery
         .Where(x => x.LocaleKey == nameof(Monitor.Name) && x.EntityId == m.Id)
         .Select(x => x.LocaleValue)
         .FirstOrDefault(),
        
         Description = monitorLocaleQuery
         .Where(x => x.LocaleKey == nameof(Monitor.Description) && x.EntityId == m.Id)
         .Select(x => x.LocaleValue)
         .FirstOrDefault(),

         Presentations =
         (from p in presentationRepository.Table.AsNoTracking()
          join sw in sensorWidgetRepository.Table.AsNoTracking() on p.SensorWidgetId equals sw.Id
          join s in sensorRepository.Table.AsNoTracking() on sw.SensorId equals s.Id
          join w in widgetRepository.Table.AsNoTracking() on sw.WidgetId equals w.Id
          join d in deviceRepository.Table.AsNoTracking() on s.DeviceId equals d.Id
          where p.MonitorId == m.Id && s.Enabled && d.Enabled
          orderby p.DisplayOrder ascending
          select new Presentation
          {
             Id = p.Id,
             DisplayOrder = p.DisplayOrder,
            
             Name = presentationLocaleQuery
                 .Where(x => x.LocaleKey == nameof(Presentation.Name) && x.EntityId == p.Id)
                 .Select(x => x.LocaleValue)
                 .FirstOrDefault(),
             
             Description = presentationLocaleQuery
                 .Where(x => x.LocaleKey == nameof(Presentation.Description) && x.EntityId == p.Id)
                 .Select(x => x.LocaleValue)
                 .FirstOrDefault(),

             Sensor = new Sensor()
             {
                Id = s.Id,
             
                Description = sensorLocaleQuery
                 .Where(x => x.LocaleKey == nameof(Sensor.Description) && x.EntityId == s.Id)
                 .Select(x => x.LocaleValue)
                 .FirstOrDefault(),
                
                DeviceId = s.DeviceId,
                SensorType = s.SensorType,
                
                MeasureUnit = sensorLocaleQuery
                 .Where(x => x.LocaleKey == nameof(Sensor.MeasureUnit) && x.EntityId == s.Id)
                 .Select(x => x.LocaleValue)
                 .FirstOrDefault(),
                
                Name = sensorLocaleQuery
                 .Where(x => x.LocaleKey == nameof(Sensor.Name) && x.EntityId == s.Id)
                 .Select(x => x.LocaleValue)
                 .FirstOrDefault(),
                
                SystemName = s.SystemName,
                PictureId = s.PictureId,
             },

             Device = new Device()
             {
                Id = d.Id,
                LastIpAddress = d.LastIpAddress,
                LastActivityOnUtc = d.LastActivityOnUtc,
                SystemName = d.SystemName,
                PictureId = d.PictureId,
                Lon = d.Lon,
                Lat = d.Lat,
                
                Name = deviceLocaleQuery
                 .Where(x => x.LocaleKey == nameof(Device.Name) && x.EntityId == s.DeviceId)
                 .Select(x => x.LocaleValue)
                 .FirstOrDefault(),
                
                Description = deviceLocaleQuery
                 .Where(x => x.LocaleKey == nameof(Device.Description) && x.EntityId == s.DeviceId)
                 .Select(x => x.LocaleValue)
                 .FirstOrDefault(),
                
                ConnectionStatus = onlineDevices.Contains(d.Id)
                 ? OnlineStatus.Online
                 : d.LastActivityOnUtc >= beenRecenlyLimit
                    ? OnlineStatus.BeenRecently
                    : d.LastActivityOnUtc != default
                       ? OnlineStatus.Offline
                       : OnlineStatus.NoActivities
             },

             Widget = new Widget()
             {
                Id = w.Id,
               
                Description = widgetLocaleQuery
                 .Where(x => x.LocaleKey == nameof(Widget.Description) && x.EntityId == w.Id)
                 .Select(x => x.LocaleValue)
                 .FirstOrDefault(),
                
                Name = widgetLocaleQuery
                 .Where(x => x.LocaleKey == nameof(Widget.Name) && x.EntityId == w.Id)
                 .Select(x => x.LocaleValue)
                 .FirstOrDefault(),
                
                PictureId = w.PictureId,
                LiveSchemePictureId = w.LiveSchemePictureId,
                Adjustment = w.Adjustment,
                WidgetType = w.WidgetType,
             }
          }).ToList()
      };

      return await query.AsSingleQuery().FirstOrDefaultAsync();
   }

   /// <summary>
   /// Get all monitor entities  by dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>The collection of devices (async operation)</returns>
   public virtual async Task<IFilterableList<Monitor>> GetAllMonitorsAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);

      var query = monitorRepository.Table.AsNoTracking();

      if (filter.UserId.HasValue)
         query = query.Where(x => x.OwnerId == filter.UserId);

      if (filter.MonitorId.HasValue)
         query = query.Where(x => x.Id == filter.MonitorId);

      if (filter.DeviceId.HasValue)
      {
         query =
         from m in query
         join p in presentationRepository.Table.AsNoTracking() on m.Id equals p.MonitorId
         join sw in sensorWidgetRepository.Table.AsNoTracking() on p.SensorWidgetId equals sw.Id
         join s in sensorRepository.Table.AsNoTracking() on sw.SensorId equals s.Id
         where s.DeviceId == filter.DeviceId
         select m;

         query = query.Distinct();
      }

      if (filter.SensorId.HasValue)
      {
         query =
         from m in query
         join p in presentationRepository.Table.AsNoTracking() on m.Id equals p.MonitorId
         join sw in sensorWidgetRepository.Table.AsNoTracking() on p.SensorWidgetId equals sw.Id
         where sw.SensorId == filter.SensorId
         select m;

         query = query.Distinct();
      }

      if (filter.WidgetId.HasValue)
      {
         query =
         from m in query
         join p in presentationRepository.Table.AsNoTracking() on m.Id equals p.MonitorId
         join sw in sensorWidgetRepository.Table.AsNoTracking() on p.SensorWidgetId equals sw.Id
         where sw.WidgetId == filter.WidgetId
         select m;

         query = query.Distinct();
      }

      if (filter.MonitorIds?.Any() ?? false)
         query = query.Where(x => filter.MonitorIds.Contains(x.Id));

      query = query.ApplyClientQuery(filter);
      query = await ProjectionQuery(filter, query);
      var result = await query.FilterAsync(filter);
      return result;
   }


   /// <summary>
   /// Get all my monitor entities by dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Collection of monitors (async operation)</returns>
   /// <remarks>For admins this returns all monitors</remarks>
   public virtual async Task<IFilterableList<Monitor>> GetOwnMonitorsAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);
      ArgumentNullException.ThrowIfNull(filter.UserId);

      var query =
      from m in monitorRepository.Table.AsNoTracking()
      where m.OwnerId == filter.UserId
      select m;

      if (filter.DeviceId.HasValue)
      {
         query =
         from m in query
         join p in presentationRepository.Table.AsNoTracking() on m.Id equals p.MonitorId
         join sw in sensorWidgetRepository.Table.AsNoTracking() on p.SensorWidgetId equals sw.Id
         join s in sensorRepository.Table.AsNoTracking() on sw.SensorId equals s.Id
         where s.DeviceId == filter.DeviceId
         select m;

         query = query.Distinct();
      }

      if (filter.SensorId.HasValue)
      {
         query =
         from m in query
         join p in presentationRepository.Table.AsNoTracking() on m.Id equals p.MonitorId
         join sw in sensorWidgetRepository.Table.AsNoTracking() on p.SensorWidgetId equals sw.Id
         where sw.SensorId == filter.SensorId
         select m;

         query = query.Distinct();
      }

      if (filter.WidgetId.HasValue)
      {
         query =
         from m in query
         join p in presentationRepository.Table.AsNoTracking() on m.Id equals p.MonitorId
         join sw in sensorWidgetRepository.Table.AsNoTracking() on p.SensorWidgetId equals sw.Id
         where sw.WidgetId == filter.WidgetId
         select m;

         query = query.Distinct();
      }

      query = await ProjectionQuery(filter, query);
      query = query.ApplyClientQuery(filter);
      var result = await query.FilterAsync(filter);
      return result;
   }

   /// <summary>
   /// Get shared monitor entities by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Collection of monitors (async operation)</returns>
   public virtual async Task<IFilterableList<Monitor>> GetSharedMonitorsAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);
      ArgumentNullException.ThrowIfNull(filter.UserId);

      var query = monitorRepository.Table.AsNoTracking();

      if (filter.MonitorId.HasValue)
         query = query.Where(x => x.Id == filter.MonitorId);

      query =
      from m in query
      join um in monitorUserRepository.Table.AsNoTracking() on m.Id equals um.MonitorId
      where um.UserId == filter.UserId
      select new Monitor()
      {
         Id = m.Id,
         AdminComment = m.AdminComment,
         CreatedOnUtc = m.CreatedOnUtc,
         Description = m.Description,
         DisplayOrder = um.DisplayOrder, //um !!!
         IsDeleted = m.IsDeleted,
         MenuItem = m.MenuItem,
         Name = m.Name,
         OwnerId = m.OwnerId,
         OwnerName = m.OwnerName,
         PictureId = m.PictureId,
         ShowInMenu = um.ShowInMenu, //um !!! 
         SubjectToAcl = m.SubjectToAcl,
         UpdatedOnUtc = m.UpdatedOnUtc
      };

      query = query.ApplyClientQuery(filter);
      query = await ProjectionQuery(filter, query);
      var result = await query.FilterAsync(filter);

      return result;
   }

   /// <summary>
   /// Get a monitor by its id for specified user
   /// </summary>
   /// <param name="id">Entity id</param>
   /// <returns>The monitor entity (async operation)</returns>
   public virtual async Task<Monitor> GetByIdAsync(long id)
   {
      if (id < 1)
         throw new ArgumentNullException(nameof(id));

      var monitor = await monitorRepository.GetByIdAsync(id, default);
      return monitor;
   }

   #endregion

   #region Update

   /// <summary>
   /// Update monitor entity
   /// </summary>
   /// <param name="monitor">Monitor entity</param>
   /// <returns>Async operation</returns>
   public virtual async Task UpdateMonitorAsync(Monitor monitor)
   {
      ArgumentNullException.ThrowIfNull(monitor);

      await monitorRepository.UpdateAsync(monitor);

      // update/insert locales
      await localizer.SaveLocaleAsync(monitor, await workContext.GetWorkingLanguageAsync());
   }

   /// <summary>
   /// Update shared monitor entity (show in menu and display order)
   /// </summary>
   /// <param name="monitor">Monitor entity</param>
   /// <param name="user">User</param>
   /// <returns>Async operation</returns>
   public virtual async Task UpdateSharedMonitorAsync(Monitor monitor, User user)
   {
      ArgumentNullException.ThrowIfNull(monitor);

      var mapping = await monitorUserRepository.Table.FirstOrDefaultAsync(x => x.MonitorId == monitor.Id && x.UserId == user.Id);

      if (mapping == null)
         return;

      mapping.ShowInMenu = monitor.ShowInMenu;
      mapping.DisplayOrder = monitor.DisplayOrder;

      await monitorUserRepository.UpdateAsync(mapping);
   }

   #endregion

   #region Insert

   /// <summary>
   /// Insert a monitor entity to DB
   /// </summary>
   /// <param name="entity">Monitor entity</param>
   /// <returns>Identifier</returns>
   public virtual async Task<long> InsertMonitorAsync(Monitor entity)
   {
      ArgumentNullException.ThrowIfNull(entity);

      await monitorRepository.InsertAsync(entity);

      // update/insert locales
      await localizer.SaveLocaleAsync(entity, await workContext.GetWorkingLanguageAsync());

      return entity.Id;
   }

   #endregion

   #region Mapping

   /// <summary>
   /// Map a monitor to a user
   /// </summary>
   /// <param name="monitorId">Monitor entity id</param>
   /// <param name="userId">User entity id</param>
   /// <returns>Async operation</returns>
   public virtual async Task ShareMonitorAsync(long monitorId, long userId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(monitorId, 1);
      ArgumentOutOfRangeException.ThrowIfLessThan(userId, 1);

      var mapping = await monitorUserRepository.Table.AsNoTracking().FirstOrDefaultAsync(x => x.MonitorId == monitorId && x.UserId == userId);

      if (mapping != null)
         return;

      await monitorUserRepository.InsertAsync(new UserMonitor()
      {
         UserId = userId,
         MonitorId = monitorId,
      });
   }

   /// <summary>
   /// Unmap a monitor to a user
   /// </summary>
   /// <param name="monitorId">Monitor entity id</param>
   /// <param name="userId">User entity id</param>
   /// <returns>Async operation</returns>
   public virtual async Task UnshareMonitorAsync(long monitorId, long userId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(monitorId, 1);
      ArgumentOutOfRangeException.ThrowIfLessThan(userId, 1);

      var mapping = await monitorUserRepository.Table.AsNoTracking().FirstOrDefaultAsync(x => x.MonitorId == monitorId && x.UserId == userId);

      if (mapping == null)
         return;

      await monitorUserRepository.DeleteAsync(mapping);
   }

   /// <summary>
   /// Is the monitor into the user scope
   /// </summary>
   /// <param name="userId">User identifier</param>
   /// <param name="monitorId">Monitor identifier</param>
   /// <returns>result: true - into the scope, false - not into the scope</returns>
   public async Task<bool> IsInUserScopeAsync(long userId, long monitorId)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(userId, 1);
      ArgumentOutOfRangeException.ThrowIfLessThan(monitorId, 1);

      var query = UserScope(userId).Where(x => x.Id == monitorId);
      return await query.AnyAsync();
   }

   #endregion

   #endregion
}
#pragma warning restore CS1591