using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Hub.Services.Clients;
using Hub.Services.Clients.Devices;
using Hub.Services.Clients.Reports;
using Hub.Services.Security;
using Hub.Services.Users;
using Hub.Web.Framework.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Shared.Clients;
using Shared.Clients.Configuration;
using Shared.Clients.Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Auto = Hub.Core.Infrastructure.Mapper.AutoMapperConfiguration;

namespace Hub.Web.Grpc.Clients;

[EnableCors(PolicyName = WebFrameworkDefaults.CorsPolicyName)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserDefaults.TelemetryRoles)]
public class DownloadTaskGrpcService : DownloadTaskRpc.DownloadTaskRpcBase
{
   #region fields

   private readonly IDownloadTaskService _downloadTaskService;
   private readonly IWorkContext _workContext;
   private readonly IUserService _userService;
   private readonly UserSettings _userSettings;
   private readonly ICommunicator _communicator;
   private readonly IDeviceService _deviceService; 

   #endregion

   #region Ctors


   public DownloadTaskGrpcService(IDownloadTaskService downloadTaskService,
      IWorkContext workContext,
      IUserService userService,
      IDeviceService deviceService,
      UserSettings userSettings, 
      ICommunicator communicator)
   {
      _downloadTaskService = downloadTaskService;
      _workContext = workContext;
      _userService = userService;
      _userSettings = userSettings;
      _deviceService = deviceService;
      _communicator = communicator;
   }


   #endregion

   #region Utilities

   /// <summary>
   /// Adds subscriptions to device message (status and others)
   /// </summary>
   /// <param name="tasks">Subscribing devices</param>
   /// <returns></returns>
   private async Task SubscribeToDownloadTaskMessages(IEnumerable<DownloadTask> tasks)
   {
      var connectionId = await _workContext.GetCurrentConncetionIdAsync();

      if (string.IsNullOrEmpty(connectionId))
         return;

      var groups = tasks.Select(x => $"{nameof(DownloadTask)}_{x.Id}");
      await _communicator.AddClientToGroupsAsync(connectionId, groups);
   }

   /// <summary>
   /// Removes subscriptions from device message (status and others)
   /// </summary>
   /// <param name="devices">Unsubscribing devices</param>
   /// <returns></returns>
   private async Task UnsubscribeFromDownloadTaskMessages(IEnumerable<DownloadTask> devices)
   {
      var connectionId = await _workContext.GetCurrentConncetionIdAsync();

      if (string.IsNullOrEmpty(connectionId))
         return;

      var groups = devices.Select(x => $"{nameof(DownloadTask)}_{x.Id}");
      await _communicator.RemoveClientFromGroupsAsync(connectionId, groups);
   }

   #endregion

   #region Methods

   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public override async Task<DownloadTaskProtos> GetAllDownloadTasks(FilterProto request, ServerCallContext context)
   {
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      var tasks = await _downloadTaskService.GetDownloadTasksAsync(filter);
      var reply = new DownloadTaskProtos();
      reply.TotalCount = tasks.TotalCount;
      var protos = Auto.Mapper.Map<List<DownloadTaskProto>>(tasks);
      reply.Items.AddRange(protos);

      return reply;
   }

   [Authorize(nameof(StandardPermissionProvider.AllowGetReports))]
   public override async Task<DownloadTaskProtos> GetDownloadTasks(FilterProto request, ServerCallContext context)
   {
      var user = await _workContext.GetCurrentUserAsync();
      var filter = Auto.Mapper.Map<DynamicFilter>(request);

      if (await _userService.IsAdminAsync(user))
         filter.UserId ??= user.Id;
      else
         filter.UserId = user.Id;

      var tasks = await _downloadTaskService.GetDownloadTasksAsync(filter);
      var reply = new DownloadTaskProtos { TotalCount = tasks.TotalCount };
      var protos = Auto.Mapper.Map<List<DownloadTaskProto>>(tasks);
      reply.Items.AddRange(protos);

      await SubscribeToDownloadTaskMessages(tasks);

      return reply;
   }

   [Authorize(nameof(StandardPermissionProvider.AllowManageReports))]
   public override async Task<DownloadTaskProtos> AddDownloadTask(DownloadRequestProto request, ServerCallContext context)
   {
      var user = await _workContext.GetCurrentUserAsync();

      if (!await _userService.IsAdminAsync(user))
      {
         var allowedTaskCount = _userSettings.AllowedDownloadTaskCount > 1 ? _userSettings.AllowedDownloadTaskCount : 1;
         if (await _downloadTaskService.GetExecutingTaskCountAsync(user) >= allowedTaskCount)
            throw new RpcException(new Status(StatusCode.Aborted, "Only one download task per user is allowed. Please, wait for the previous tasks will have been done."));
      }

      var download = Auto.Mapper.Map<DownloadRequest>(request);

      var now = DateTime.UtcNow;
      var downloadTask = new DownloadTask()
      {
         CurrentState = DownloadFileState.InTheQueue,
         TaskDateTimeUtc = now,
         UserId = user.Id,
         FileName = download.Compression == FileCompressionType.None
            ? $"report{user.Id}_{now.Ticks}.{download.Format.ToString().ToLower()}"
            : $"report{user.Id}_{now.Ticks}.{download.Format.ToString().ToLower()}.{download.Compression.ToString().ToLower()}",

         QueryString = JsonSerializer.Serialize(new
         {
            download.From,
            download.To,
            download.DeviceId,
            download.SensorIds,
            download.Format,
            download.Compression
         }),
      };

      await _downloadTaskService.InsertDownloadTaskAsync(downloadTask);

      var filter = new DynamicFilter() { Top = download.Top, UserId = user.Id, OrderBy= "TaskDateTimeUtc desc" };
      var tasks = await _downloadTaskService.GetDownloadTasksAsync(filter);

      var reply = new DownloadTaskProtos();
      var protos = Auto.Mapper.Map<List<DownloadTaskProto>>(tasks);
      reply.Items.AddRange(protos);
      reply.TotalCount = tasks.TotalCount;

      await SubscribeToDownloadTaskMessages(tasks);

      return reply;
   }

   [Authorize(nameof(StandardPermissionProvider.AllowManageReports))]
   public override async Task<Empty> DeleteDownloadTask(IdProto request, ServerCallContext context)
   {
      if(request.Id < 1)
         throw new ArgumentOutOfRangeException(nameof(request.Id));

      var user = await _workContext.GetCurrentUserAsync();

      var deletingTask = await _downloadTaskService.GetDownloadTaskByIdAsync(request.Id);

      if (deletingTask != null)
      {
         if (!await _userService.IsAdminAsync(user) && deletingTask.UserId != user.Id)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "You cannot delete not your own dowload tasks"));

         if(deletingTask.CurrentState != DownloadFileState.InTheQueue)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "The download task is already precessing or canceled."));

         await _downloadTaskService.DeleteDownloadTaskAsync(deletingTask);
         await UnsubscribeFromDownloadTaskMessages(new[] { deletingTask });
      }

      return new Empty();
   }

   #endregion
}
