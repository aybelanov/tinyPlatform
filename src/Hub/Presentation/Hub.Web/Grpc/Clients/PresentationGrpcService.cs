using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hub.Core;
using Hub.Services.Clients.Monitors;
using Hub.Services.Clients.Sensors;
using Hub.Services.Clients.Widgets;
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
using System.Threading.Tasks;
using Auto = Hub.Core.Infrastructure.Mapper.AutoMapperConfiguration;


namespace Hub.Web.Grpc.Clients;

[EnableCors(PolicyName = WebFrameworkDefaults.CorsPolicyName)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserDefaults.TelemetryRoles)]
public class PresentationGrpcService(IWorkContext workContext,
   IUserService userService,
   IMonitorService monitorService,
   IPresentationService presentationService,
   ISensorService sensorService,
   IWidgetService widgetService)
   : PresentationRpc.PresentationRpcBase
{
   #region fields

   private readonly IWorkContext _workContext = workContext;
   private readonly IUserService _userService = userService;
   private readonly IMonitorService _monitorService = monitorService;
   private readonly IPresentationService _presentationService = presentationService;
   private readonly ISensorService _sensorService = sensorService;
   private readonly IWidgetService _widgetService = widgetService;

   #endregion

   #region Methods

   [Authorize(nameof(StandardPermissionProvider.AllowGetData))]
   public override async Task<PresentationProtos> GetPresentations(FilterProto request, ServerCallContext context)
   {
      ArgumentNullException.ThrowIfNull(request.MonitorId);
      ArgumentOutOfRangeException.ThrowIfLessThan(request.MonitorId.Value, 1);

      var user = await _workContext.GetCurrentUserAsync();

      if (!await _userService.IsAdminAsync(user))
         request.UserId = user.Id;

      var filter = Auto.Mapper.Map<DynamicFilter>(request);

      var presentations = await _presentationService.GetPresentationsAsync(filter);
      var reply = new PresentationProtos() { TotalCount = presentations.TotalCount };
      reply.Presentations.AddRange(Auto.Mapper.Map<List<PresentationProto>>(presentations));

      return reply;
   }


   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public override async Task<PresentationSelectItemProtos> GetAllPresentationSelectItems(FilterProto request, ServerCallContext context)
   {
      var filter = Auto.Mapper.Map<DynamicFilter>(request);

      var items = await _presentationService.GetAllPresentationSelectListAsync(filter);
      var protos = new PresentationSelectItemProtos { TotalCount = items.TotalCount };
      protos.Presentations.AddRange(Auto.Mapper.Map<List<PresentationSelectItemProto>>(items));

      return protos;
   }


   [Authorize(nameof(StandardPermissionProvider.AllowManageWidgets))]
   public override async Task<PresentationSelectItemProtos> GetOwnPresentationSelectItems(FilterProto request, ServerCallContext context)
   {
      var user = await _workContext.GetCurrentUserAsync();
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      filter.UserId = user.Id;

      var items = await _presentationService.GetOwnPresentationSelectListAsync(filter);
      var protos = new PresentationSelectItemProtos { TotalCount = items.TotalCount };
      protos.Presentations.AddRange(Auto.Mapper.Map<List<PresentationSelectItemProto>>(items));

      return protos;
   }


   [Authorize(nameof(StandardPermissionProvider.AllowManageMonitors))]
   public override async Task<PresentationProto> MapPresentation(PresentationProto request, ServerCallContext context)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(request.MonitorId, 1);
      ArgumentOutOfRangeException.ThrowIfLessThan(request.SensorWidgetId, 1);

      var user = await _workContext.GetCurrentUserAsync();

      if (!await _userService.IsAdminAsync(user) && !await _presentationService.IsSensorWidgetInUserScopeAsync(user.Id, request.SensorWidgetId))
         throw new RpcException(new Status(StatusCode.PermissionDenied, "You cannot map not your own monitors and presentations."));

      var presentation = Auto.Mapper.Map<Core.Domain.Clients.Presentation>(request);
      await _presentationService.MapPresentationAsync(presentation);

      var response = Auto.Mapper.Map<PresentationProto>(presentation);
      return response;
   }


   [Authorize(nameof(StandardPermissionProvider.AllowManageMonitors))]
   public override async Task<PresentationProto> UpdateMapPresentation(PresentationProto request, ServerCallContext context)
   {
      var presentation = await _presentationService.GetPresentationByIdAsync(request.Id)
         ?? throw new RpcException(new Status(StatusCode.Aborted, "Presentation doesn't exist."));

      var user = await _workContext.GetCurrentUserAsync();
      if (!await _userService.IsAdminAsync(user) && !await _monitorService.IsInUserScopeAsync(user.Id, presentation.MonitorId))
         throw new RpcException(new Status(StatusCode.PermissionDenied, "You cannot unmap not your own monitors"));

      Auto.Mapper.Map(request, presentation);
      await _presentationService.UpdateMapPresentationAsync(presentation);

      var response = Auto.Mapper.Map<PresentationProto>(presentation);
      return response;
   }


   [Authorize(nameof(StandardPermissionProvider.AllowManageMonitors))]
   public override async Task<Empty> UnmapPresentation(IdProto request, ServerCallContext context)
   {
      var presentation = await _presentationService.GetPresentationByIdAsync(request.Id);

      if (presentation == null)
         return new Empty();

      var user = await _workContext.GetCurrentUserAsync();
      if (!await _userService.IsAdminAsync(user) && !await _monitorService.IsInUserScopeAsync(user.Id, presentation.MonitorId))
         throw new RpcException(new Status(StatusCode.PermissionDenied, "You cannot unmap not your own monitors"));

      await _presentationService.UnmapPresentationAsync(presentation);

      return new();
   }


   [Authorize(nameof(StandardPermissionProvider.AllowManageWidgets))]
   public override async Task<Empty> MapSensorToWidget(FilterProto request, ServerCallContext context)
   {
      ArgumentNullException.ThrowIfNull(request.SensorId);
      ArgumentNullException.ThrowIfNull(request.WidgetId);
      ArgumentOutOfRangeException.ThrowIfLessThan(request.SensorId.Value, 1);
      ArgumentOutOfRangeException.ThrowIfLessThan(request.WidgetId.Value, 1);

      var user = await _workContext.GetCurrentUserAsync();
      if (!await _userService.IsAdminAsync(user)
         && (!await _widgetService.IsInUserScopeAsync(user.Id, request.WidgetId.Value) || !await _sensorService.IsInUserScopeAsync(user.Id, request.SensorId.Value)))
         throw new RpcException(new Status(StatusCode.PermissionDenied, "You cannot add not your own mapping"));

      await _presentationService.MapSensorToWidgetAsync(request.SensorId.Value, request.WidgetId.Value);

      return new();
   }


   [Authorize(nameof(StandardPermissionProvider.AllowManageWidgets))]
   public override async Task<Empty> UnmapSensorFromWidget(FilterProto request, ServerCallContext context)
   {
      ArgumentNullException.ThrowIfNull(request.SensorId);
      ArgumentNullException.ThrowIfNull(request.WidgetId);
      ArgumentOutOfRangeException.ThrowIfLessThan(request.SensorId.Value, 1);
      ArgumentOutOfRangeException.ThrowIfLessThan(request.WidgetId.Value, 1);

      var user = await _workContext.GetCurrentUserAsync();
      if (!await _userService.IsAdminAsync(user)
         && (!await _widgetService.IsInUserScopeAsync(user.Id, request.WidgetId.Value) || !await _sensorService.IsInUserScopeAsync(user.Id, request.SensorId.Value)))
         throw new RpcException(new Status(StatusCode.PermissionDenied, "You cannot remove not your own mapping"));

      await _presentationService.UnmapSensorFromWidgetAsync(request.SensorId.Value, request.WidgetId.Value);

      return new();
   }

   #endregion
}