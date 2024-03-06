using Hub.Core.Caching;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Hub.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Reports;

/// <summary>
/// Represents a download task service implementation
/// </summary>
public class DownloadTaskService : IDownloadTaskService
{
   #region fields

   private readonly IRepository<DownloadTask> _downloadTasksRepositiry;
   private readonly IRepository<User> _userRepository;
   private readonly UserSettings _userSettings;
   private readonly IStaticCacheManager _staticCacheManager;

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public DownloadTaskService(IRepository<DownloadTask> downloadTasksRepositiry,
      IRepository<User> userRepository,
      UserSettings userSettings,
      IStaticCacheManager staticCacheManager)
   {
      _downloadTasksRepositiry = downloadTasksRepositiry;
      _userRepository = userRepository;
      _userSettings = userSettings;
      _staticCacheManager = staticCacheManager;
   }


   #endregion

   #region Methods

   /// <summary>
   /// Delete a download task
   /// </summary>
   /// <param name="downloadTask">Deleting download task</param>
   /// <returns></returns>
   public async Task DeleteDownloadTaskAsync(DownloadTask downloadTask)
   {
      ArgumentNullException.ThrowIfNull(downloadTask);
      await _downloadTasksRepositiry.DeleteAsync(downloadTask);
   }

   /// <summary>
   /// Delete a download task by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns></returns>
   public async Task DeleteDownloadTaskAsync(DynamicFilter filter)
   {
      ArgumentNullException.ThrowIfNull(filter);
      var tasks = _downloadTasksRepositiry.Table.ApplyClientQuery(filter);
      await LinqToDB.LinqExtensions.DeleteAsync(tasks);
   }

   /// <summary>
   /// Gets the last added download task 
   /// </summary>
   /// <returns>Download task</returns>
   public async Task<DownloadTask> GetNextDownloadTaskAsync()
   {
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(ClientCacheDefaults<DownloadTask>.ByLastEntityCacheKey);

      var query = _downloadTasksRepositiry.Table
         .Where(x => x.CurrentState == DownloadFileState.InTheQueue && x.DelayUntilUtc <= DateTime.UtcNow)
         .OrderByDescending(x => x.TaskDateTimeUtc);

      return await _staticCacheManager.GetAsync(cacheKey,
         // some trick to use cache
         // its a stub if there are no task with "in the queue" state (null)
         async () => await query.FirstOrDefaultAsync() ?? new() { Id = 0 });
   }

   /// <summary>
   /// Gets a download task by its identifier
   /// </summary>
   /// <param name="downloadTaskId">Download task identifier</param>
   /// <returns>Download task entity</returns>
   public async Task<DownloadTask> GetDownloadTaskByIdAsync(long downloadTaskId)
   {
      var task = await _downloadTasksRepositiry.Table.Where(x => x.Id == downloadTaskId).FirstOrDefaultAsync();
      return task;
   }

   /// <summary>
   /// Gets download tasks by the dynamic filter 
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Downloa task collection</returns>
   public async Task<IFilterableList<DownloadTask>> GetDownloadTasksAsync(DynamicFilter filter)
   {
      var query = _downloadTasksRepositiry.Table.AsNoTracking();

      // user query
      var userQuery = _userRepository.Table.AsNoTracking();

      if (filter.UserId.HasValue)
         userQuery = userQuery.Where(x => x.Id == filter.UserId);

      // filter user by owner name column
      if (filter.AdditionalQueries.TryGetValue(nameof(User), out var additionalUserQuery) && !string.IsNullOrEmpty(additionalUserQuery))
      {
         additionalUserQuery = additionalUserQuery.Replace("Username", _userSettings.UsernamesEnabled ? nameof(User.Username) : nameof(User.Email));
         userQuery = userQuery.Where(additionalUserQuery);
      }

      query =
         from t in query
         join u in userQuery on t.UserId equals u.Id
         select new DownloadTask()
         {
            Id = t.Id,
            CurrentStateId = t.CurrentStateId,
            CurrentState = t.CurrentState,
            FileName = t.FileName,
            QueryString = t.QueryString,
            ReadyDateTimeUtc = t.ReadyDateTimeUtc,
            TaskDateTimeUtc = t.TaskDateTimeUtc,
            Size = t.Size,
            UserId = t.UserId,
            Username = _userSettings.UsernamesEnabled ? u.Username : u.Email
         };

      var result = await query.FilterAsync(filter);
      return result;
   }

   /// <summary>
   /// Gets executing download tasks by the user
   /// </summary>
   /// <param name="user">User</param>
   /// <returns>Task count</returns>
   public async Task<int> GetExecutingTaskCountAsync(User user)
   {
      ArgumentNullException.ThrowIfNull(user);

      var count = await _downloadTasksRepositiry.Table.AsNoTracking()
         .Where(x => x.UserId == user.Id && (x.CurrentState == DownloadFileState.Processing || x.CurrentState == DownloadFileState.InTheQueue))
         .CountAsync();

      return count;
   }

   /// <summary>
   /// Inserts a dowload task to the database
   /// </summary>
   /// <param name="downloadTask">Adding download task</param>
   /// <returns>Assigned id</returns>
   public async Task<long> InsertDownloadTaskAsync(DownloadTask downloadTask)
   {
      ArgumentNullException.ThrowIfNull(downloadTask);
      await _downloadTasksRepositiry.InsertAsync(downloadTask);
      return downloadTask.Id;
   }

   /// <summary>
   /// Updates a download task
   /// </summary>
   /// <param name="downloadTask">Updating download task</param>
   /// <returns></returns>
   public async Task UpdateDownloadTaskAsync(DownloadTask downloadTask)
   {
      ArgumentNullException.ThrowIfNull(downloadTask);
      await _downloadTasksRepositiry.UpdateAsync(downloadTask);
   }

   #endregion
}
