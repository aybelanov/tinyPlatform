using Clients.Dash.Domain;
using Clients.Dash.Models;
using Clients.Dash.Services.EntityServices;
using Clients.Dash.Services.Localization;
using Clients.Dash.Services.Security;
using Microsoft.AspNetCore.Components;
using Shared.Clients;
using System;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;

namespace Clients.Dash.Pages.Reports.Export;

/// <summary>
/// Component partial class
/// </summary>
public partial class DownloadTaskTable
{

   [Inject] IDownloadTaskService DownloadTaskService { get; set; }
   [Inject] PermissionService PermissionService { get; set; }
   [Inject] ISensorService SensorService { get; set; }
   [Inject] IDeviceService DeviceService { get; set; }
   [Inject] Localizer Localizer { get; set; }


   /// <summary>
   /// Prepares a download task models by the download task request
   /// </summary>
   /// <param name="request">Download task request</param>
   /// <returns>Download task model collection</returns>
   public async Task<IFilterableList<DownloadTaskModel>> PrepareDownloadTaskModelAsync(DownloadRequest request)
   {
      var tasks = await DownloadTaskService.AddDownloadTaskAsync(request);
      var models = Auto.Mapper.Map<FilterableList<DownloadTaskModel>>(tasks);
      models.TotalCount = tasks.TotalCount;

      return models;
   }


   /// <summary>
   /// Prepares a download task models by the dynamic filter
   /// </summary>
   /// <param name="filter">Dynamic filter</param>
   /// <returns>Download task model collection</returns>
   public async Task<IFilterableList<DownloadTaskModel>> PrepareDownloadTaskModelAsync(DynamicFilter filter)
   {
      Func<DynamicFilter, Task<IFilterableList<DownloadTask>>> getData = await PermissionService.IsAdminModeAsync()
         ? DownloadTaskService.GetAllDownloadTasksAsync
         : DownloadTaskService.GetDownloadTasksAsync;

      var tasks = await getData(filter);
      var models = Auto.Mapper.Map<FilterableList<DownloadTaskModel>>(tasks);
      models.TotalCount = tasks.TotalCount;

      return models;
   }

   /// <summary>
   /// Prepares a download task models from a download task entity
   /// </summary>
   /// <param name="task">Download task entity</param>
   /// <returns>Download task model</returns>
   public Task<DownloadTaskModel> PrepareDownloadTaskModelAsync(DownloadTask task)
   {
      var model = Auto.Mapper.Map<DownloadTaskModel>(task);

      return Task.FromResult(model);
   }


   /// <summary>
   /// Data file download task
   /// </summary>
   public class DownloadTaskModel : BaseEntityModel, IDownloadTask
   {
      /// <summary>
      /// User identifier
      /// </summary>
      public long UserId { get; set; }

      /// <summary>
      /// Username
      /// </summary>
      public string Username { get; set; }

      /// <summary>
      /// File full name
      /// </summary>
      public string FileName { get; set; }

      /// <summary>
      /// Download task creation datetime
      /// </summary>
      public DateTime TaskDateTimeUtc { get; set; }

      /// <summary>
      /// File readiness datetime on UTC
      /// </summary>
      public DateTime ReadyDateTimeUtc { get; set; }

      /// <summary>
      /// Current file readiness state
      /// </summary>
      public DownloadFileState CurrentState
      {
         get => (DownloadFileState)CurrentStateId;
         set => CurrentStateId = (int)value;
      }

      /// <summary>
      /// Current file readiness state identifier
      /// </summary>
      public int CurrentStateId { get; set; }

      /// <summary>
      /// File size
      /// </summary>
      public long Size { get; set; }
   }
}
