using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Services.Clients;
using Hub.Services.Clients.Sensors;
using Hub.Services.Common;
using Hub.Services.Devices;
using Hub.Services.Localization;
using Hub.Services.Media;
using Hub.Services.Security;
using Hub.Services.Users;
using Hub.Web.Framework.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json.Linq;
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
public class SensorGrpcService(IWorkContext workContext,
   ISensorService sensorService,
   IHubSensorService hubSensorService,
   IUserService userService,
   IGenericAttributeService genericAttributeService,
   ILocalizationService localizationService,
   IPictureService pictureService,
   IHubDeviceService hubDeviceService) : SensorRpc.SensorRpcBase
{

   #region Utils

   private Task<bool> IsConfigurationChanged(SensorProto request, Sensor sensor) 
   {
      var result = 
      !(string.IsNullOrWhiteSpace(request.Configuration) && string.IsNullOrWhiteSpace(sensor.Configuration))
      || JToken.DeepEquals(request.Configuration?.Trim() ?? "{}", sensor.Configuration?.Trim() ?? "{}")
      || request.SystemName != sensor.SystemName
      || request.Enabled != sensor.Enabled
      || request.PriorityType != (int)sensor.PriorityType;

      return Task.FromResult(result);
   }

   #endregion

   #region Methods

   [Authorize( Roles = UserDefaults.TelemetryAdminRoles)]
   public override async Task<Empty> Delete(IdProto request, ServerCallContext context)
   {
      var sensor = await hubSensorService.GetSensorByIdAsync(request.Id);
      if (sensor is null)
         return new Empty();

      var user = await workContext.GetCurrentUserAsync();
      var device = await hubDeviceService.GetDeviceByIdAsync(sensor.DeviceId);

      if (device.OwnerId != user.Id && !await userService.IsAdminAsync(user))
         throw new RpcException(new(StatusCode.PermissionDenied, "You cannot delete not your own sensor."));

      await sensorService.DeleteAsync(sensor);

      return new Empty();
   }


   [Authorize(nameof(StandardPermissionProvider.AllowGetData))]
   public override async Task<SensorProtos> GetSensors(FilterProto request, ServerCallContext context)
   {     
      var user = await workContext.GetCurrentUserAsync();
      
      // security
      if(!await userService.IsAdminAsync(user))
         request.UserId = user.Id;

      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      var sensors = await sensorService.GetSensorsAsync(filter);
      var protos = new SensorProtos() { TotalCount = sensors.TotalCount };

      foreach (var sensor in sensors)
      {
         var proto = Auto.Mapper.Map<SensorProto>(sensor);
         proto.PictureUrl = await pictureService.GetPictureUrlAsync(sensor.PictureId);
         protos.Sensors.Add(proto);
      }

      return protos;
   }

   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public override async Task<SensorSelectItemProtos> GetForAllSensorSelectList(FilterProto request, ServerCallContext context)
   {
      ArgumentNullException.ThrowIfNull(request.DeviceId);

      var filter = Auto.Mapper.Map<DynamicFilter>(request);

      var items = await sensorService.GetSensorSelectItemListAsync(filter);
      var reply = new SensorSelectItemProtos();
      reply.Sensors.AddRange(Auto.Mapper.Map<List<SensorSelectItemProto>>(items));
      reply.TotalCount = items.TotalCount;

      return reply;
   }

   [Authorize(nameof(StandardPermissionProvider.AllowGetData))]
   public override async Task<SensorSelectItemProtos> GetSensorSelectList(FilterProto request, ServerCallContext context)
   {
      ArgumentNullException.ThrowIfNull(request.DeviceId);

      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      var user = await workContext.GetCurrentUserAsync();
      filter.UserId = user.Id;

      var items = await sensorService.GetSensorSelectItemListAsync(filter);
      var reply = new SensorSelectItemProtos();
      reply.Sensors.AddRange(Auto.Mapper.Map<List<SensorSelectItemProto>>(items));
      reply.TotalCount = items.TotalCount;

      return reply;
   }

   [Authorize(Roles = UserDefaults.TelemetryAdminRoles)]
   public override async Task<SensorProto> Insert(SensorProto request, ServerCallContext context)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(request.DeviceId, 1);

      var user = await workContext.GetCurrentUserAsync();

      var device = await hubDeviceService.GetDeviceByIdAsync(request.DeviceId) 
         ?? throw new RpcException(new(StatusCode.NotFound, "Device not found"));

      if (device.OwnerId != user.Id && !await userService.IsAdminAsync(user))
         throw new RpcException(new(StatusCode.PermissionDenied, "It's not your device."));

      var sensor = Auto.Mapper.Map<Sensor>(request);
      await sensorService.InsertAsync(sensor);

      // save device configuration for further syncronization with the dispatcher
      await genericAttributeService.SaveAttributeAsync(device, ClientDefaults.DeviceConfigurationVersion, DateTime.UtcNow.Ticks);

      var sensorProto = Auto.Mapper.Map<SensorProto>(sensor);
      sensorProto.PictureUrl = await pictureService.GetPictureUrlAsync(sensor.PictureId);

      return sensorProto;
   }

   [Authorize( Roles = UserDefaults.TelemetryAdminRoles)]
   public override async Task<SensorProto> Update(SensorProto request, ServerCallContext context)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(request.DeviceId, 1);

      var user = await workContext.GetCurrentUserAsync();

      var device = await hubDeviceService.GetDeviceByIdAsync(request.DeviceId)
          ?? throw new RpcException(new(StatusCode.NotFound, "Device not found"));

      if (device.OwnerId != user.Id && !await userService.IsAdminAsync(user))
         throw new RpcException(new(StatusCode.PermissionDenied, "It's not your device."));

      var sensor = await hubSensorService.GetSensorByIdAsync(request.Id)
         ?? throw new RpcException(new(StatusCode.NotFound, "Sensor not found"));

      if(sensor.DeviceId != device.Id)
         throw new RpcException(new(StatusCode.Aborted, "Bad request"));

      var isConfigChanged = await IsConfigurationChanged(request, sensor);

      Auto.Mapper.Map(request, sensor);
      await sensorService.UpdateAsync(sensor);

      if(isConfigChanged)
         await genericAttributeService.SaveAttributeAsync(device, ClientDefaults.DeviceConfigurationVersion, DateTime.UtcNow.Ticks);

      var response = Auto.Mapper.Map<SensorProto>(sensor);
      response.PictureUrl = await pictureService.GetPictureUrlAsync(sensor.PictureId);

      return response;
   }

   [Authorize( Roles = UserDefaults.TelemetryAdminRoles)]
   public override async Task<CommonResponse> CheckSystemNameAvailability(SystemNameAvailabilityRequest request, ServerCallContext context)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(request.SystemName);
      ArgumentNullException.ThrowIfNull(request.Id);
      ArgumentOutOfRangeException.ThrowIfLessThan(request.Id.Value, 1);

      var sensor = await hubSensorService.GetSensorBySystemNameAsync(request.SystemName, request.Id.Value);

      var response = new CommonResponse();

      if (sensor is not null)
         response.Error = await localizationService.GetResourceAsync("Sensors.Fields.SystemName.AlreadyExist");

      return response;
   }

   #endregion
}
