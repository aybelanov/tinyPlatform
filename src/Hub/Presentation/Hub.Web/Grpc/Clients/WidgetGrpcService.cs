using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Services.Clients.Widgets;
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
using System.Collections.Generic;
using System.Threading.Tasks;
using Auto = Hub.Core.Infrastructure.Mapper.AutoMapperConfiguration;

namespace Hub.Web.Grpc.Clients;

[EnableCors(PolicyName = WebFrameworkDefaults.CorsPolicyName)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserDefaults.TelemetryRoles)]
public class WidgetGrpcService(IWidgetService widgetService,
   IUserService userService,
   IWorkContext workContext,
   IPictureService pictureService)
   : WidgetRpc.WidgetRpcBase
{

   #region Methods

   [Authorize(nameof(StandardPermissionProvider.AllowManageWidgets))]
   public override async Task<Empty> Delete(IdProto request, ServerCallContext context)
   {
      var widget = await widgetService.GetByIdAsync(request.Id);
      if (widget is null)
         return new Empty();

      var user = await workContext.GetCurrentUserAsync();

      // security only admins can delete not your own monitors 
      if (user.Id != widget.UserId && !await userService.IsAdminAsync(user))
         throw new RpcException(new(StatusCode.Aborted, "You cannot delete not your own widget."));

      await widgetService.DeleteAsync(widget);

      return new();
   }

   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public override async Task<WidgetProtos> GetAllWidgets(FilterProto request, ServerCallContext context)
   {
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      var widgets = await widgetService.GetAllWidgetsAsync(filter);

      var protos = new WidgetProtos();
      protos.TotalCount = widgets.TotalCount;

      foreach (var widget in widgets)
      {
         var proto = Auto.Mapper.Map<WidgetProto>(widget);
         proto.PictureUrl = await pictureService.GetPictureUrlAsync(widget.PictureId);

         if (widget.WidgetType == WidgetType.LiveScheme && widget.LiveSchemePictureId > 0)
            proto.LiveSchemeUrl = await pictureService.GetPictureUrlAsync(widget.LiveSchemePictureId);

         protos.Widgets.Add(proto);
      }

      return protos;
   }

   [Authorize(nameof(StandardPermissionProvider.AllowManageWidgets))]
   public override async Task<WidgetProtos> GetOwnWidgets(FilterProto request, ServerCallContext context)
   {
      var user = await workContext.GetCurrentUserAsync();
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      filter.UserId = user.Id;

      var widgets = await widgetService.GetAllWidgetsAsync(filter);

      var protos = new WidgetProtos();
      protos.TotalCount = widgets.TotalCount;

      foreach (var widget in widgets)
      {
         var proto = Auto.Mapper.Map<WidgetProto>(widget);
         proto.PictureUrl = await pictureService.GetPictureUrlAsync(widget.PictureId);

         if (widget.WidgetType == WidgetType.LiveScheme && widget.LiveSchemePictureId > 0)
            proto.LiveSchemeUrl = await pictureService.GetPictureUrlAsync(widget.LiveSchemePictureId);

         protos.Widgets.Add(proto);
      }

      return protos;
   }

   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public override async Task<WidgetSelectItemProtos> GetAllWidgetSelectItems(FilterProto request, ServerCallContext context)
   {
      var filter = Auto.Mapper.Map<DynamicFilter>(request);

      var items = await widgetService.GetAllWidgetSelectListAsync(filter);
      var protos = new WidgetSelectItemProtos { TotalCount = items.TotalCount };
      protos.Widgets.AddRange(Auto.Mapper.Map<List<WidgetSelectItemProto>>(items));

      return protos;
   }


   [Authorize(nameof(StandardPermissionProvider.AllowManageWidgets))]
   public override async Task<WidgetSelectItemProtos> GetOwnWidgetSelectItems(FilterProto request, ServerCallContext context)
   {
      var user = await workContext.GetCurrentUserAsync();
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      filter.UserId = user.Id;

      var items = await widgetService.GetOwnWidgetSelectListAsync(filter);
      var protos = new WidgetSelectItemProtos { TotalCount = items.TotalCount };
      protos.Widgets.AddRange(Auto.Mapper.Map<List<WidgetSelectItemProto>>(items));

      return protos;
   }

   [Authorize(nameof(StandardPermissionProvider.AllowManageWidgets))]
   public override async Task<WidgetProto> Insert(WidgetProto request, ServerCallContext context)
   {
      var user = await workContext.GetCurrentUserAsync();

      if (request.UserId == 0 || !await userService.IsAdminAsync(user))
         request.UserId = user.Id;

      var widget = Auto.Mapper.Map<Widget>(request);
      await widgetService.InsertAsync(widget);

      var widgetProto = Auto.Mapper.Map<WidgetProto>(widget);
      widgetProto.PictureUrl = await pictureService.GetPictureUrlAsync(widget.PictureId);

      if (widget.WidgetType == WidgetType.LiveScheme && widget.LiveSchemePictureId > 0)
         widgetProto.LiveSchemeUrl = await pictureService.GetPictureUrlAsync(widget.LiveSchemePictureId);

      return widgetProto;
   }

   [Authorize(nameof(StandardPermissionProvider.AllowManageWidgets))]
   public override async Task<WidgetProto> Update(WidgetProto request, ServerCallContext context)
   {
      var user = await workContext.GetCurrentUserAsync();

      var isAdmiin = await userService.IsAdminAsync(user);

      if (request.UserId != user.Id && !isAdmiin)
         throw new RpcException(new Status(StatusCode.Aborted, "You cannot update not your own widget."));

      var widget = await widgetService.GetByIdAsync(request.Id)
         ?? throw new RpcException(new Status(StatusCode.Aborted, "Widget is not exist"));

      if (widget.UserId != user.Id && !isAdmiin)
         throw new RpcException(new(StatusCode.Aborted, "You cannot update not your own widget."));

      Auto.Mapper.Map(request, widget);
      await widgetService.UpdateAsync(widget);

      var response = Auto.Mapper.Map<WidgetProto>(widget);
      response.PictureUrl = await pictureService.GetPictureUrlAsync(widget.PictureId);

      if (widget.WidgetType == Shared.Clients.Domain.WidgetType.LiveScheme && widget.LiveSchemePictureId < 1)
         response.LiveSchemeUrl = await pictureService.GetPictureUrlAsync(widget.LiveSchemePictureId);

      return response;
   }

   #endregion
}
