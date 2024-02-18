using Clients.Dash.Domain;
using Clients.Dash.Services.Security;
using Grpc.Core;
using Shared.Clients;
using Shared.Clients.Proto;
using System;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;

namespace Clients.Dash.Services.EntityServices.Grpc;

/// <summary>
/// Represents a download data file service implementation
/// </summary>
public class DownloadTaskGrpcService : IDownloadTaskService
{
   #region fields

   private readonly DownloadTaskRpc.DownloadTaskRpcClient _client;

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public DownloadTaskGrpcService(DownloadTaskRpc.DownloadTaskRpcClient client)
   {
      _client = client;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Adds a download task to the download queue
   /// </summary>
   /// <param name="request">Download task request</param>
   /// <returns>Download task collection</returns>
   public async Task<IFilterableList<DownloadTask>> AddDownloadTaskAsync(DownloadRequest request)
   {
      var requestProto = Auto.Mapper.Map<DownloadRequestProto>(request);
      var taskProtos = await _client.AddDownloadTaskAsync(requestProto);
      var tasks = Auto.Mapper.Map<FilterableList<DownloadTask>>(taskProtos.Items);
      tasks.TotalCount = taskProtos.TotalCount ?? 0;
      
      return tasks;
   }

   /// <summary>
   /// Deletes a download task by the identifier
   /// </summary>
   /// <param name="id">Download task identifier</param>
   /// <returns></returns>
   public async Task DeleteDownloadTaskAsync(long id)
   {
      if (id < 1)
         throw new ArgumentOutOfRangeException();

      await _client.DeleteDownloadTaskAsync(new() { Id = id });
   }

   /// <summary>
   /// Gets download tasks by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Download task collection</returns>
   public async Task<IFilterableList<DownloadTask>> GetDownloadTasksAsync(DynamicFilter filter)
   {     
      var filterProto = Auto.Mapper.Map<FilterProto>(filter);
      var taskProtos = await _client.GetDownloadTasksAsync(filterProto);
      var tasks = Auto.Mapper.Map<FilterableList<DownloadTask>>(taskProtos.Items);
      tasks.TotalCount = taskProtos.TotalCount ?? 0;

      return tasks;
   }

   /// <summary>
   /// Gets all download tasks by the dynamic filter (for admin mode only)
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Download task collection</returns>
   public async Task<IFilterableList<DownloadTask>> GetAllDownloadTasksAsync(DynamicFilter filter)
   {
      var filterProto = Auto.Mapper.Map<FilterProto>(filter);
      var taskProtos = await _client.GetAllDownloadTasksAsync(filterProto);
      var tasks = Auto.Mapper.Map<FilterableList<DownloadTask>>(taskProtos.Items);
      tasks.TotalCount = taskProtos.TotalCount ?? 0;

      return tasks;
   }

   #endregion
}
