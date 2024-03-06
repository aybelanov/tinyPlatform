using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Shared.Clients;
using System.Threading.Tasks;

namespace Hub.Services.Clients.Reports;

/// <summary>
/// Represents a download task service interface
/// </summary>
public interface IDownloadTaskService
{
   /// <summary>
   /// Delete a download task
   /// </summary>
   /// <param name="downloadTask">Deleting download task</param>
   /// <returns></returns>
   Task DeleteDownloadTaskAsync(DownloadTask downloadTask);

   /// <summary>
   /// Delete a download task by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns></returns>
   Task DeleteDownloadTaskAsync(DynamicFilter filter);

   /// <summary>
   /// Gets download tasks by the dynamic filter 
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Downloa task collection</returns>
   Task<IFilterableList<DownloadTask>> GetDownloadTasksAsync(DynamicFilter filter);

   /// <summary>
   /// Gets a download task by its identifier
   /// </summary>
   /// <param name="downloadTaskId">Download task identifier</param>
   /// <returns>Download task entity</returns>
   Task<DownloadTask> GetDownloadTaskByIdAsync(long downloadTaskId);

   /// <summary>
   /// Inserts a dowload task to the database
   /// </summary>
   /// <param name="downloadTask">Adding download task</param>
   /// <returns>Assigned id</returns>
   Task<long> InsertDownloadTaskAsync(DownloadTask downloadTask);

   /// <summary>
   /// Updates a download task
   /// </summary>
   /// <param name="downloadTask">Updating download task</param>
   /// <returns></returns>
   Task UpdateDownloadTaskAsync(DownloadTask downloadTask);

   /// <summary>
   /// Gets executing download tasks by the user
   /// </summary>
   /// <param name="user">User</param>
   /// <returns>Task count</returns>
   Task<int> GetExecutingTaskCountAsync(User user);

   /// <summary>
   /// Gets the last added download task 
   /// </summary>
   /// <returns>Download task</returns>
   Task<DownloadTask> GetNextDownloadTaskAsync();
}
