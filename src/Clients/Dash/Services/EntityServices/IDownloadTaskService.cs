using Clients.Dash.Domain;
using Shared.Clients;
using System.Threading.Tasks;

namespace Clients.Dash.Services.EntityServices;

/// <summary>
/// Represents a download data file service interface
/// </summary>
public interface IDownloadTaskService
{
   /// <summary>
   /// Adds a download task to the download queue
   /// </summary>
   /// <param name="request">Download task request</param>
   /// <returns>Download task collection</returns>
   Task<IFilterableList<DownloadTask>> AddDownloadTaskAsync(DownloadRequest request);

   /// <summary>
   /// Gets download tasks by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Download task collection</returns>
   Task<IFilterableList<DownloadTask>> GetDownloadTasksAsync(DynamicFilter filter);

   /// <summary>
   /// Gets all download tasks by the dynamic filter (for admin mode only)
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Download task collection</returns>
   Task<IFilterableList<DownloadTask>> GetAllDownloadTasksAsync(DynamicFilter filter);

   /// <summary>
   /// Deletes a download task by the identifier
   /// </summary>
   /// <param name="id">Download task identifier</param>
   /// <returns></returns>
   Task DeleteDownloadTaskAsync(long id);
}
