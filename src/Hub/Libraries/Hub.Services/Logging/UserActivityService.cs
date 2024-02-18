using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Users;
using Hub.Core.Domain.Logging;
using Hub.Data;
using Shared.Common;
using Shared.Clients;

namespace Hub.Services.Logging;

/// <summary>
/// User activity service
/// </summary>
public class UserActivityService : BaseActivityService, IUserActivityService
{
   #region Fields

   private readonly IRepository<ActivityLog> _activityLogRepository;
   private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
   private readonly IWebHelper _webHelper;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public UserActivityService(IRepository<ActivityLog> activityLogRepository,
       IRepository<ActivityLogType> activityLogTypeRepository,
       IWebHelper webHelper,
       IWorkContext workContext)
      : base(activityLogRepository, activityLogTypeRepository, webHelper)
   {
      _activityLogRepository = activityLogRepository;
      _webHelper = webHelper;
      _workContext = workContext;
      _activityLogTypeRepository = activityLogTypeRepository;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Gets all activity log type items
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the activity log type items
   /// </returns>
   public override async Task<IList<ActivityLogType>> GetAllActivityTypesAsync()
   {
      var activityLogTypes = (await base.GetAllActivityTypesAsync()).Where(x => !x.SystemKeyword.StartsWith("Device."));

      return activityLogTypes.ToList();
   }

   /// <summary>
   /// Inserts an activity log item
   /// </summary>
   /// <param name="systemKeyword">System keyword</param>
   /// <param name="comment">Comment</param>
   /// <param name="entity">Entity</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the activity log item
   /// </returns>
   public virtual async Task<ActivityLog> InsertActivityAsync(string systemKeyword, string comment, BaseEntity entity = null)
   {
      return await InsertActivityAsync(await _workContext.GetCurrentUserAsync(), systemKeyword, comment, entity);
   }


   /// <summary>
   /// Inserts an activity log item
   /// </summary>
   /// <param name="user">User</param>
   /// <param name="systemKeyword">System keyword</param>
   /// <param name="comment">Comment</param>
   /// <param name="entity">Entity</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the activity log item
   /// </returns>
   public virtual async Task<ActivityLog> InsertActivityAsync(User user, string systemKeyword, string comment, BaseEntity entity = null)
   {
      if (user == null)
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
         SubjectId = user.Id,
         SubjectName = typeof(User).Name,
         Comment = CommonHelper.EnsureMaximumLength(comment ?? string.Empty, 4000),
         CreatedOnUtc = DateTime.UtcNow,
         IpAddress = _webHelper.GetCurrentIpAddress()
      };
      await _activityLogRepository.InsertAsync(logItem);

      return logItem;
   }


   /// <summary>
   /// Gets all activity log items
   /// </summary>
   /// <param name="createdOnFrom">Log item creation from; pass null to load all records</param>
   /// <param name="createdOnTo">Log item creation to; pass null to load all records</param>
   /// <param name="userId">User identifier; pass null to load all records</param>
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
   public virtual async Task<IPagedList<ActivityLog>> GetAllActivitiesAsync(
       DateTime? createdOnFrom = null,
       DateTime? createdOnTo = null,
       long? userId = null,
       long? activityLogTypeId = null,
       string ipAddress = null,
       string entityName = null,
       long? entityId = null,
       int pageIndex = 0,
       int pageSize = int.MaxValue)
   {
      return await base.GetAllActivitiesAsync(
         createdOnFrom,
         createdOnTo,
         userId,
         typeof(User).Name,
         activityLogTypeId,
         ipAddress,
         entityName,
         entityId,
         pageIndex,
         pageSize);
   }


   /// <summary>
   /// Clears user activity log
   /// </summary>
   /// <returns>A task that represents the asynchronous operation</returns>
   public override async Task ClearAllActivitiesAsync()
   {
      await base.ClearAllActivitiesAsync<User>();
   }

   /// <summary>
   /// Gets user activities by the dynamic filter
   /// </summary>
   /// <param name="filter">Dinamic filter</param>
   /// <returns>FIlterable collection of the activity records</returns>
   public async Task<IFilterableList<ActivityLog>> GetActivitiesByDynamicFilterAsync(DynamicFilter filter)
   {
      var result = await GetActivitiesByDynamicFilterAsync(nameof(User), filter);
      return result;
   }

   #endregion
}