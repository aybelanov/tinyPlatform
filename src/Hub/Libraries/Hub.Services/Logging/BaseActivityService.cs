using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Users;
using Hub.Core.Domain.Logging;
using Hub.Data;
using Shared.Common;
using Microsoft.EntityFrameworkCore;
using Hub.Core.Domain.Clients;
using Hub.Services.Clients;
using Shared.Clients;
using Shared.Clients.Proto;

namespace Hub.Services.Logging;

/// <summary>
/// User activity service
/// </summary>
public class BaseActivityService 
{
   #region Fields

   private readonly IRepository<ActivityLog> _activityLogRepository;
   private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
   private readonly IWebHelper _webHelper;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public BaseActivityService(IRepository<ActivityLog> activityLogRepository,
       IRepository<ActivityLogType> activityLogTypeRepository,
       IWebHelper webHelper)
   {
      _activityLogRepository = activityLogRepository;
      _activityLogTypeRepository = activityLogTypeRepository;
      _webHelper = webHelper;
   }

   #endregion

   #region Methods


   /// <summary>
   /// Updates an activity log type item
   /// </summary>
   /// <param name="activityLogType">Activity log type item</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task UpdateActivityTypeAsync(ActivityLogType activityLogType)
   {
      await _activityLogTypeRepository.UpdateAsync(activityLogType);
   }

   /// <summary>
   /// Gets all activity log type items
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the activity log type items
   /// </returns>
   public virtual async Task<IList<ActivityLogType>> GetAllActivityTypesAsync()
   {
      var activityLogTypes = await _activityLogTypeRepository.GetAllAsync(query =>
      {
         return from alt in query
                orderby alt.Name
                select alt;
      }, cache => default);

      return activityLogTypes;
   }

   /// <summary>
   /// Gets an activity log type item
   /// </summary>
   /// <param name="activityLogTypeId">Activity log type identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the activity log type item
   /// </returns>
   public virtual async Task<ActivityLogType> GetActivityTypeByIdAsync(long activityLogTypeId)
   {
      return await _activityLogTypeRepository.GetByIdAsync(activityLogTypeId, cache => default);
   }

 
   /// <summary>
   /// Inserts an activity log item
   /// </summary>
   /// <param name="subject">Log subject</param>
   /// <param name="systemKeyword">System keyword</param>
   /// <param name="comment">Comment</param>
   /// <param name="entity">Entity</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the activity log item
   /// </returns>
   public virtual async Task<ActivityLog> InsertActivityAsync(BaseEntity subject, string systemKeyword, string comment, BaseEntity entity = null)
   {
      if (subject == null)
         return null;

      //try to get activity log type by passed system keyword
      var activityLogType = (await GetAllActivityTypesAsync()).FirstOrDefault(type => type.SystemKeyword.Equals(systemKeyword));
      if (!activityLogType?.Enabled ?? true)
         return null;

      //insert log item
      var logItem = new ActivityLog
      {
         ActivityLogTypeId = activityLogType.Id,
         EntityId = entity?.Id,
         EntityName = entity?.GetType().Name,
         SubjectId = subject.Id,
         SubjectName = subject.GetType().Name,
         Comment = CommonHelper.EnsureMaximumLength(comment ?? string.Empty, 4000),
         CreatedOnUtc = DateTime.UtcNow,
         IpAddress = _webHelper.GetCurrentIpAddress()
      };
      await _activityLogRepository.InsertAsync(logItem);

      return logItem;
   }

   /// <summary>
   /// Deletes an activity log item
   /// </summary>
   /// <param name="activityLog">Activity log type</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteActivityAsync(ActivityLog activityLog)
   {
      await _activityLogRepository.DeleteAsync(activityLog);
   }

   /// <summary>
   /// Gets all activity log items
   /// </summary>
   /// <param name="createdOnFrom">Log item creation from; pass null to load all records</param>
   /// <param name="createdOnTo">Log item creation to; pass null to load all records</param>
   /// <param name="subjectId">Subject identifier; pass null to load all records</param>
   /// <param name="subjectName">Subject type name; pass null to load all records</param>
   /// <param name="activityLogTypeId">Activity log type identifier; pass null to load all records</param>
   /// <param name="ipAddress">IP address; pass null or empty to load all records</param>
   /// <param name="entityName">Entity name; pass null to load all records</param>
   /// <param name="entityId">Entity identifier; pass null to load all records</param>
   /// <param name="pageIndex">Page index</param>
   /// <param name="pageSize">Page size</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the activity log items
   /// </returns>
   public virtual async Task<IPagedList<ActivityLog>> GetAllActivitiesAsync(DateTime? createdOnFrom = null, DateTime? createdOnTo = null,
       long? subjectId = null, string subjectName = null, long? activityLogTypeId = null, string ipAddress = null, string entityName = null,
       long? entityId = null, int pageIndex = 0, int pageSize = int.MaxValue)
   {
      return await _activityLogRepository.GetAllPagedAsync(_ =>
      {
         var query = _activityLogRepository.Table.AsNoTracking();

         //filter by IP
         if (!string.IsNullOrEmpty(ipAddress))
            query = query.Where(logItem => logItem.IpAddress.Contains(ipAddress));

         //filter by creation date
         if (createdOnFrom.HasValue)
            query = query.Where(logItem => createdOnFrom.Value <= logItem.CreatedOnUtc);
         if (createdOnTo.HasValue)
            query = query.Where(logItem => createdOnTo.Value >= logItem.CreatedOnUtc);

         //filter by log type
         if (activityLogTypeId.HasValue && activityLogTypeId.Value > 0)
            query = query.Where(logItem => activityLogTypeId == logItem.ActivityLogTypeId);

         //filter record by subject and logtype keyword
         if (!string.IsNullOrEmpty(subjectName))
         {
            query = subjectName == typeof(Device).Name
            ? query.Where(logItem => logItem.SubjectName.Equals(typeof(Device).Name))
            : query.Where(logItem => !logItem.SubjectName.Equals(typeof(Device).Name));
         }   

         //filter by subject identifier
         if (subjectId.HasValue && subjectId.Value > 0)
            query = query.Where(logItem => subjectId.Value == logItem.SubjectId);

         //filter by entity
         if (!string.IsNullOrEmpty(entityName))
            query = query.Where(logItem => logItem.EntityName.Equals(entityName));
         if (entityId.HasValue && entityId.Value > 0)
            query = query.Where(logItem => entityId.Value == logItem.EntityId);

         query = query.OrderByDescending(logItem => logItem.CreatedOnUtc).ThenBy(logItem => logItem.Id);

         return query;

      }, pageIndex, pageSize);
   }

   /// <summary>
   /// Get last activity record for an entity
   /// </summary>
   /// <param name="subject">Subject entity</param>
   /// <param name="systemKeyword">Activity log type system keword</param>
   /// <returns>Activity log record</returns>
   public virtual async Task<ActivityLog> GetLastActivityRecordAsync(BaseEntity subject = null, string systemKeyword = null)
   {
      var query = _activityLogRepository.Table.AsNoTracking();

      if(subject != null)
         query  = query.Where(x=>x.SubjectId == subject.Id && x.SubjectName == subject.GetType().Name);

      if (!string.IsNullOrWhiteSpace(systemKeyword))
         query = from al in query
                 join alt in _activityLogTypeRepository.Table.AsNoTracking() on al.ActivityLogTypeId equals alt.Id
                 where alt.SystemKeyword == systemKeyword
                 select al;

      query = query.OrderByDescending(x => x.CreatedOnUtc);
      var result = await query.FirstOrDefaultAsync();   

      return result;
   }

   /// <summary>
   /// Gets an activity log item
   /// </summary>
   /// <param name="activityLogId">Activity log identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the activity log item
   /// </returns>
   public virtual async Task<ActivityLog> GetActivityByIdAsync(long activityLogId)
   {
      return await _activityLogRepository.GetByIdAsync(activityLogId);
   }

   /// <summary>
   /// Clears activity log
   /// </summary>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task ClearAllActivitiesAsync()
   {
      await _activityLogRepository.TruncateAsync();
   }

   /// <summary>
   /// Clears activity log for a subject (User or Device etc.)
   /// </summary>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task ClearAllActivitiesAsync<T>()
   {
      await _activityLogRepository.DeleteAsync(query => query.SubjectName == typeof(T).Name);
   }


   /// <summary>
   /// Gets activities by the dynamic filter
   /// </summary>
   /// <param name="filter">Dinamic filter</param>
   /// <param name="entityName">Entity name</param>
   /// <returns>FIlterable collection of the activity records</returns>
   public virtual async Task<IFilterableList<ActivityLog>> GetActivitiesByDynamicFilterAsync(string entityName, DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter, nameof(filter));
      var query = _activityLogRepository.Table.Where(x => x.SubjectName == entityName).AsNoTracking();

      if (filter.DeviceId.HasValue)
         query = query.Where(x => x.SubjectId == filter.DeviceId);

      if (filter.UserId.HasValue)
         query = query.Where(x => x.SubjectId == filter.UserId);

      query =
         from r in query
         join t in _activityLogTypeRepository.Table.AsNoTracking() on r.ActivityLogTypeId equals t.Id
         select new ActivityLog()
         {
            Id = r.Id,
            ActivityType = t.Name,
            Comment = r.Comment,
            CreatedOnUtc = r.CreatedOnUtc,
            EntityName = r.EntityName,
            IpAddress = r.IpAddress   
         };

      var result = await query.FilterAsync(filter);
      return result;
   }

   #endregion
}