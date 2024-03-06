using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Hub.Services.Clients.Monitors;
using Hub.Services.Media;
using Hub.Services.Security;
using Hub.Services.Users;
using Hub.Web.Framework.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Shared.Clients;
using Shared.Clients.Configuration;
using Shared.Clients.Domain;
using Shared.Clients.Proto;
using System.Linq;
using System.Threading.Tasks;
using Auto = Hub.Core.Infrastructure.Mapper.AutoMapperConfiguration;

namespace Hub.Web.Grpc.Clients;

[EnableCors(PolicyName = WebFrameworkDefaults.CorsPolicyName)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserDefaults.TelemetryRoles)]
public class MonitorGrpcService(IMonitorService monitorService,
   IPictureService pictureService,
   UserSettings userSettings,
   IWorkContext workContext,
   IUserService userService) : MonitorRpc.MonitorRpcBase
{

   #region Methods

   #region MonitorView

   [Authorize(nameof(StandardPermissionProvider.AllowGetData))]
   public override async Task<MonitorViewProto> GetMonitorView(IdProto request, ServerCallContext context)
   {
      var user = await workContext.GetCurrentUserAsync();

      var view = await monitorService.GetMonitorViewAsync(request.Id, await userService.IsAdminAsync(user) ? null : user.Id);

      if (view?.Presentations is not null) foreach (var p in view.Presentations)
         {
            p.Sensor.PictureUrl = await pictureService.GetPictureUrlAsync(p.Sensor.PictureId);
            p.Device.PictureUrl = await pictureService.GetPictureUrlAsync(p.Device.PictureId);
            p.Widget.PictureUrl = await pictureService.GetPictureUrlAsync(p.Widget.PictureId);

            if (p.Widget.WidgetType == WidgetType.LiveScheme)
               p.Widget.LiveSchemeUrl = await pictureService.GetPictureUrlAsync(p.Widget.LiveSchemePictureId);
         }

      var reply = Auto.Mapper.Map<MonitorViewProto>(view);
      return reply;
   }

   #endregion

   #region Monitor



   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public override async Task<MonitorProtos> GetAllMonitors(FilterProto request, ServerCallContext context)
   {
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      var monitors = await monitorService.GetAllMonitorsAsync(filter);

      var protos = new MonitorProtos();
      protos.TotalCount = monitors.TotalCount;

      foreach (var monitor in monitors)
      {
         var proto = Auto.Mapper.Map<MonitorProto>(monitor);
         //proto.PictureUrl = await _pictureService.GetPictureUrlAsync(monitor.PictureId, 50);
         protos.Monitors.Add(proto);
      }

      return protos;
   }


   [Authorize(nameof(StandardPermissionProvider.AllowGetData))]
   public override async Task<MonitorProtos> GetOwnMonitors(FilterProto request, ServerCallContext context)
   {
      var user = await workContext.GetCurrentUserAsync();
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      filter.UserId = user.Id;

      var monitors = await monitorService.GetOwnMonitorsAsync(filter);

      var protos = new MonitorProtos();
      protos.TotalCount = monitors.TotalCount;

      foreach (var monitor in monitors)
      {
         var proto = Auto.Mapper.Map<MonitorProto>(monitor);
         //proto.PictureUrl = await _pictureService.GetPictureUrlAsync(monitor.PictureId, 50);
         protos.Monitors.Add(proto);
      }

      return protos;
   }


   [Authorize(nameof(StandardPermissionProvider.AllowManageMonitors))]
   public override async Task<MonitorProto> Insert(MonitorProto request, ServerCallContext context)
   {
      var user = await workContext.GetCurrentUserAsync();

      if (request.OwnerId == 0 || !await userService.IsAdminAsync(user))
         request.OwnerId = user.Id;

      var monitor = Auto.Mapper.Map<Monitor>(request);
      await monitorService.InsertMonitorAsync(monitor);

      var response = Auto.Mapper.Map<MonitorProto>(monitor);
      return response;
   }


   [Authorize(nameof(StandardPermissionProvider.AllowManageMonitors))]
   public override async Task<MonitorProto> Update(MonitorProto request, ServerCallContext context)
   {
      var user = await workContext.GetCurrentUserAsync();

      var isAdmiin = await userService.IsAdminAsync(user);

      if (request.OwnerId != user.Id && !isAdmiin)
         throw new RpcException(new Status(StatusCode.Aborted, "You cannot update not your own monitor."));

      var monitor = await monitorService.GetByIdAsync(request.Id)
         ?? throw new RpcException(new Status(StatusCode.Aborted, "Monitor is not exist"));

      if (monitor.OwnerId != user.Id && !isAdmiin)
         throw new RpcException(new(StatusCode.Aborted, "You cannot update not your own monitor."));

      Auto.Mapper.Map(request, monitor);
      await monitorService.UpdateMonitorAsync(monitor);

      var response = Auto.Mapper.Map<MonitorProto>(monitor);
      return response;
   }


   [Authorize(nameof(StandardPermissionProvider.AllowManageMonitors))]
   public override async Task<Empty> Delete(IdProto request, ServerCallContext context)
   {
      var monitor = await monitorService.GetByIdAsync(request.Id);
      if (monitor is null)
         return new Empty();

      var user = await workContext.GetCurrentUserAsync();

      // security only admins can delete not your own monitors 
      if (user.Id != monitor.OwnerId && !await userService.IsAdminAsync(user))
         throw new RpcException(new(StatusCode.Aborted, "You cannot delete not your own monitor."));

      await monitorService.DeleteAsync(monitor);

      return new();
   }

   #endregion

   #region Sharing

   [Authorize(nameof(StandardPermissionProvider.AllowGetData))]
   public override async Task<MonitorProtos> GetSharedMonitors(FilterProto request, ServerCallContext context)
   {
      var user = await workContext.GetCurrentUserAsync();
      var filter = Auto.Mapper.Map<DynamicFilter>(request);

      // only admins can get not their shared monitors
      if (await userService.IsAdminAsync(user))
         filter.UserId ??= user.Id;
      else
         filter.UserId = user.Id;

      var monitors = await monitorService.GetSharedMonitorsAsync(filter);

      var protos = new MonitorProtos();
      protos.TotalCount = monitors.TotalCount;

      foreach (var monitor in monitors)
      {
         var proto = Auto.Mapper.Map<MonitorProto>(monitor);
         //proto.PictureUrl = await _pictureService.GetPictureUrlAsync(monitor.PictureId, 50);
         protos.Monitors.Add(proto);
      }

      return protos;
   }

   [Authorize(nameof(StandardPermissionProvider.AllowManageMonitors))]
   public override async Task<Empty> DeleteShared(MonitorProto request, ServerCallContext context)
   {
      // security
      var user = await workContext.GetCurrentUserAsync();
      var unmappingMonitor = (await monitorService.GetSharedMonitorsAsync(new() { UserId = user.Id, MonitorId = request.Id })).FirstOrDefault();

      if (unmappingMonitor is not null)
         await monitorService.UnshareMonitorAsync(unmappingMonitor.Id, user.Id);

      return new Empty();
   }

   [Authorize(nameof(StandardPermissionProvider.AllowManageMonitors))]
   public override async Task<Empty> ShareMonitor(ShareRequest request, ServerCallContext context)
   {
      var subject = await workContext.GetCurrentUserAsync();

      if (!await userService.IsAdminAsync(subject) && !await monitorService.IsInUserScopeAsync(subject.Id, request.EntityId))
         throw new RpcException(new(StatusCode.PermissionDenied, "You cannot share not your own monitor"));

      var user = userSettings.UsernamesEnabled
         ? await userService.GetUserByUsernameAsync(request.UserName)
         : await userService.GetUserByEmailAsync(request.UserName);

      if (user == null || user.IsDeleted || !user.IsActive)
         throw new RpcException(new(StatusCode.Aborted, "User does not exist or blocked"));

      if (!await userService.IsAdminAsync(user) && !await userService.IsOperatorAsync(user) && !await userService.IsOwnerAsync(user))
         throw new RpcException(new(StatusCode.Aborted, "You cannot share the monitor to not telemetry user."));

      await monitorService.ShareMonitorAsync(request.EntityId, user.Id);

      return new();
   }

   public override async Task<Empty> UnshareMonitor(ShareRequest request, ServerCallContext context)
   {
      var subject = await workContext.GetCurrentUserAsync();

      if (!await userService.IsAdminAsync(subject) && !await monitorService.IsInUserScopeAsync(subject.Id, request.EntityId))
         throw new RpcException(new(StatusCode.PermissionDenied, "You cannot stop sharing not your own monitor"));

      var user = userSettings.UsernamesEnabled
         ? await userService.GetUserByUsernameAsync(request.UserName)
         : await userService.GetUserByEmailAsync(request.UserName);

      if (user is not null)
         await monitorService.UnshareMonitorAsync(request.EntityId, user.Id);

      return new();
   }


   [Authorize(nameof(StandardPermissionProvider.AllowManageMonitors))]
   public override async Task<Empty> UpdateShared(MonitorProto request, ServerCallContext context)
   {
      // security
      var user = await workContext.GetCurrentUserAsync();
      var updatingMonitor = (await monitorService.GetSharedMonitorsAsync(new() { UserId = user.Id, MonitorId = request.Id })).FirstOrDefault();

      if (updatingMonitor is not null)
      {
         updatingMonitor.ShowInMenu = request.ShowInMenu;
         updatingMonitor.DisplayOrder = request.DisplayOrder;
         await monitorService.UpdateSharedMonitorAsync(updatingMonitor, user);
      }

      return new Empty();
   }

   #endregion

   #endregion
}
