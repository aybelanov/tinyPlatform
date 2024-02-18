using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Users;
using Hub.Services.Clients;
using Hub.Services.Clients.Devices;
using Hub.Services.Common;
using Hub.Services.Devices;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Media;
using Hub.Services.Security;
using Hub.Services.Users;
using Hub.Web.Framework.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Shared.Clients;
using Shared.Clients.Configuration;
using Shared.Clients.Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auto = Hub.Core.Infrastructure.Mapper.AutoMapperConfiguration;

namespace Hub.Web.Grpc.Clients;

[EnableCors(PolicyName = WebFrameworkDefaults.CorsPolicyName)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserDefaults.TelemetryRoles)]
public class DeviceGrpcService(IDeviceService deviceService,
  IHubDeviceService hubDeviceService,
  IPictureService pictureService,
  IWorkContext workContext,
  IGenericAttributeService gaService,
  ILocalizationService localizationService,
  ICommunicator communicator,
  DeviceSettings deviceSettings,
  UserSettings userSettings,
  IDeviceActivityService deviceActivityService,
  IHttpContextAccessor httpContextAccessor,
  IDeviceRegistrationService deviceRegistrationService,
  IUserService userService) : DeviceRpc.DeviceRpcBase
{
   #region fields

   private readonly IDeviceService _deviceService = deviceService;
   private readonly IHubDeviceService _hubDeviceService = hubDeviceService;
   private readonly IPictureService _pictureService = pictureService;
   private readonly IWorkContext _workContext = workContext;
   private readonly ICommunicator _communicator = communicator;   
   private readonly DeviceSettings _deviceSettings = deviceSettings;
   private readonly UserSettings _userSettings = userSettings;
   private readonly IUserService _userService = userService;
   private readonly IGenericAttributeService _gaService = gaService;
   private readonly IDeviceRegistrationService _deviceRegistrationService = deviceRegistrationService;
   private readonly ILocalizationService _localizationService = localizationService;
   private readonly IDeviceActivityService _deviceActivityService = deviceActivityService;

   #endregion

   #region Utilities

   /// <summary>
   /// Checks fieds to determine device configuration changing
   /// </summary>
   /// <param name="request">Inbound request</param>
   /// <param name="device">Checking device</param>
   /// <returns>Bool result (true, false)</returns>
   private bool IsConfigurationChanged(DeviceProto request, Device device)
   {
      return !request.SystemName.Equals(device.SystemName)
          || request.ClearDataDelay != device.ClearDataDelay
          || request.CountDataRows != device.CountDataRows
          || request.DataPacketSize != device.DataPacketSize
          || request.DataSendingDelay != device.DataSendingDelay
          || request.Configuration != device.Configuration;
   }

   /// <summary>
   /// Adds subscriptions to device message (status and others)
   /// </summary>
   /// <param name="devices">Subscribing devices</param>
   /// <returns></returns>
   private async Task SubscribeToDeviceMessages(IEnumerable<Device> devices)
   {
      // add subscriptions to device message (status and others)
      var connectionId = await _workContext.GetCurrentConncetionIdAsync();
     
      if (string.IsNullOrEmpty(connectionId))
         return;
     
      var groups = devices.Select(x => $"{nameof(Device)}_{x.Id}");
      await _communicator.AddClientToGroupsAsync(connectionId, groups);
   }

   /// <summary>
   /// Removes subscriptions from device message (status and others)
   /// </summary>
   /// <param name="devices">Unsubscribing devices</param>
   /// <returns></returns>
   private async Task UnsubscribeFromDeviceMessages(IEnumerable<Device> devices)
   {
      // add subscriptions to device message (status and others)
      var connectionId = await _workContext.GetCurrentConncetionIdAsync();

      if (string.IsNullOrEmpty(connectionId))
         return;

      var groups = devices.Select(x => $"{nameof(Device)}_{x.Id}");
      await _communicator.RemoveClientFromGroupsAsync(connectionId, groups);
   }

   /// <summary>
   /// Adds coomon data to device entities: pictures, IP addres and etc.
   /// </summary>
   /// <param name="devices">Device colletion</param>
   /// <returns>Device proto collection</returns>
   private async Task<DeviceProtos> PrepareDeviceProtosAsync(IFilterableList<Device> devices)
   {
      var protos = new DeviceProtos { TotalCount = devices.TotalCount };

      foreach (var device in devices)
      {
         var proto = Auto.Mapper.Map<DeviceProto>(device);
         proto.PictureUrl = await _pictureService.GetPictureUrlAsync(device.PictureId);

         proto.LastIpAddress = _deviceSettings.StoreIpAddresses
            ? (await _communicator.GetDeviceCallContextAsync(device.Id))?.GetHttpContext()?.Connection.RemoteIpAddress.MapToIPv4().ToString()
            : HubCommonDefaults.SpoofedIp;

         protos.Devices.Add(proto);
      }

      return protos;
   }

   #endregion

   #region Methods

   [Authorize(nameof(StandardPermissionProvider.AllowManageDevices))]
   public override async Task<Empty> Delete(IdProto request, ServerCallContext context)
   {
      var user = await _workContext.GetCurrentUserAsync();
      var device = await _hubDeviceService.GetDeviceByIdAsync(request.Id);

      if(device == null)
         return new Empty();

      if (user.Id != device.OwnerId && !await _userService.IsAdminAsync(user))
         throw new RpcException(new(StatusCode.Aborted, "You cannot delete not your own device."));

      await _deviceService.DeleteAsync(device);

      await UnsubscribeFromDeviceMessages(new[] { device });

      return new();
   }

   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public override async Task<DeviceProtos> GetAllDevices(FilterProto request, ServerCallContext context)
   {
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      var devices = await _deviceService.GetAllDevicesAsync(filter);

      var protos = await PrepareDeviceProtosAsync(devices);
      await SubscribeToDeviceMessages(devices);

      return protos;
   }

   [Authorize(nameof(StandardPermissionProvider.AllowGetData))]
   public override async Task<DeviceProtos> GetOwnDevices(FilterProto request, ServerCallContext context)
   {
      var user = await _workContext.GetCurrentUserAsync();
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      filter.UserId = user.Id;   

      var devices = await _deviceService.GetOwnDevicesAsync(filter);
      var protos = await PrepareDeviceProtosAsync(devices);

      await SubscribeToDeviceMessages(devices);

      return protos;
   }

   [Authorize(nameof(StandardPermissionProvider.AllowGetData))]
   public override async Task<DeviceProtos> GetSharedDevices(FilterProto request, ServerCallContext context)
   {
      var user = await _workContext.GetCurrentUserAsync();
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      filter.UserId = user.Id;

      //// only admins can get not their shared devices
      //if (await _userService.IsAdminAsync(user))
      //   filter.UserId ??= user.Id;
      //else
      //   filter.UserId = user.Id;

      var devices = await _deviceService.GetSharedDevicesAsync(filter);
      var protos = await PrepareDeviceProtosAsync(devices);

      await SubscribeToDeviceMessages(devices);

      return protos;
   }

   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public override async Task<DeviceMapItems> GetAllMapDevice(FilterProto request, ServerCallContext context)
   {
      var filter = Auto.Mapper.Map<DynamicFilter>(request);

      var items = await _deviceService.GetAllDeviceMapItemsAsync(filter);
      var protos = new DeviceMapItems() { TotalCount = items.TotalCount };
      protos.Items.AddRange(Auto.Mapper.Map<List<DeviceMapItemProto>>(items));

      return protos;
   }

   [Authorize(nameof(StandardPermissionProvider.AllowGetData))]
   public override async Task<DeviceMapItems> GetUserMapDevice(FilterProto request, ServerCallContext context)
   {
      var user = await _workContext.GetCurrentUserAsync();
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      filter.UserId = user.Id;

      var items = await _deviceService.GetUserDeviceMapItemsAsync(filter);
      var protos = new DeviceMapItems() { TotalCount = items.TotalCount };
      protos.Items.AddRange(Auto.Mapper.Map<List<DeviceMapItemProto>>(items));

      return protos;
   }

   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public override async Task<DeviceSelectItemProtos> GetAllDeviceSelectList(FilterProto request, ServerCallContext context)
   {
      var filter = Auto.Mapper.Map<DynamicFilter>(request);

      var items = await _deviceService.GetAllDeviceSelectListAsync(filter);
      var protos = new DeviceSelectItemProtos { TotalCount = items.TotalCount };
      protos.Devices.AddRange(Auto.Mapper.Map<List<DeviceSelectItemProto>>(items));

      return protos;
   }

   [Authorize(nameof(StandardPermissionProvider.AllowGetData))]
   public override async Task<DeviceSelectItemProtos> GetUserDeviceSelectList(FilterProto request, ServerCallContext context)
   {
      var user = await _workContext.GetCurrentUserAsync();
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      filter.UserId = user.Id;

      var items = await _deviceService.GetUserDeviceSelectListAsync(filter);
      var protos = new DeviceSelectItemProtos { TotalCount = items.TotalCount };
      protos.Devices.AddRange(Auto.Mapper.Map<List<DeviceSelectItemProto>>(items));

      return protos;
   }

   [Authorize(nameof(StandardPermissionProvider.AllowManageDevices))]
   public override async Task<DeviceProto> Insert(DeviceProto request, ServerCallContext context)
   {
      var user = await _workContext.GetCurrentUserAsync();

      if (request.OwnerId == 0 || !await _userService.IsAdminAsync(user))
         request.OwnerId = user.Id;

      //fill entity from model
      var device = Auto.Mapper.Map<Device>(request);
      var registrationRequest = new DeviceRegistrationRequest(device, request.SystemName, request.Password, _deviceSettings.DefaultPasswordFormat, true);
      var registerResult = await _deviceRegistrationService.RegisterDeviceAsync(registrationRequest);

      if (registerResult.Success)
      {
         // activity log
         await _deviceActivityService.InsertActivityAsync(device, "Device.Register", $"Device {device.SystemName} has been registered by {user.Email}.", user);

         // save device configuration for further syncronization with the dispatcher
         await _gaService.SaveAttributeAsync(device, ClientDefaults.DeviceConfigurationVersion, DateTime.UtcNow.Ticks);
         
         var deviceProto = Auto.Mapper.Map<DeviceProto>(device);
         deviceProto.PictureUrl = await _pictureService.GetPictureUrlAsync(device.PictureId);
         
         await SubscribeToDeviceMessages(new[] { device });

         return deviceProto;
      }
      else
      {
         throw new RpcException(new Status(StatusCode.InvalidArgument, string.Join("; ", registerResult.Errors)));
      }
   }


   [Authorize(nameof(StandardPermissionProvider.AllowManageDevices))]
   public override async Task<DeviceProto> Update(DeviceProto request, ServerCallContext context)
   {
      var user = await _workContext.GetCurrentUserAsync();
      var isAdmiin = await _userService.IsAdminAsync(user);

      if (request.OwnerId != user.Id && !isAdmiin)
         throw new RpcException(new Status(StatusCode.Aborted, "You cannot update not your own devices."));

      var device = await _hubDeviceService.GetDeviceByIdAsync(request.Id)
         ?? throw new RpcException(new Status(StatusCode.Aborted, "Device is not exist"));

      if (device.OwnerId != user.Id && !isAdmiin)
         throw new RpcException(new(StatusCode.Aborted, "You cannot update not your own devices."));

      DeviceRegistrationResult validateResult;
      if (!device.SystemName.Equals(request.SystemName) && !(validateResult = await _deviceRegistrationService.ValidateSystemNameAsync(request.SystemName)).Success)
         throw new RpcException(new Status(StatusCode.Aborted, string.Join("; ", validateResult.Errors)));

      var isConfigurationChanged = IsConfigurationChanged(request, device);

      device = Auto.Mapper.Map(request, device);
      await _deviceService.UpdateDeviceAsync(device);

      // check configuration changing
      if (isConfigurationChanged)
         await _gaService.SaveAttributeAsync(device, ClientDefaults.DeviceConfigurationVersion, DateTime.UtcNow.Ticks);

      var response = Auto.Mapper.Map<DeviceProto>(device);
      response.PictureUrl = await _pictureService.GetPictureUrlAsync(device.PictureId);

      return response;
   }


   [Authorize(nameof(StandardPermissionProvider.AllowManageDevices))]
   public override async Task<Empty> ChangePassword(ChangeDevicePassword request, ServerCallContext context)
   {
      ArgumentOutOfRangeException.ThrowIfLessThan(request.DeviceId, 1);
      ArgumentException.ThrowIfNullOrWhiteSpace(request.Password);

      var user = await _workContext.GetCurrentUserAsync();

      var device = await _hubDeviceService.GetDeviceByIdAsync(request.DeviceId)
         ?? throw new RpcException(new(StatusCode.NotFound, "Device not found"));

      if (device.OwnerId != user.Id && !await _userService.IsAdminAsync(user))
         throw new RpcException(new(StatusCode.PermissionDenied, "It's not your device."));

      var changePassRequest = new ChangePasswordRequest(device.SystemName, false, _deviceSettings.DefaultPasswordFormat, request.Password);
      var changePassResult = await _deviceRegistrationService.ChangePasswordAsync(changePassRequest);

      if (!changePassResult.Success)
         throw new RpcException(new Status(StatusCode.Cancelled, string.Join("; ", changePassResult.Errors)), string.Join("; ", changePassResult.Errors));

      // configuration changing
      await _gaService.SaveAttributeAsync(device, ClientDefaults.DeviceConfigurationVersion, DateTime.UtcNow.Ticks);

      return new Empty();
   }


   [Authorize(nameof(StandardPermissionProvider.AllowManageDevices))]
   public override async Task<CommonResponse> CheckPasswordFormat(PasswordFormatRequest request, ServerCallContext context)
   {
      var validateResult = await _deviceRegistrationService.ValidatePasswordFormatAsync(request.Password);
      return new() { Error = string.Join("; ", validateResult.Errors) };
   }


   [Authorize(nameof(StandardPermissionProvider.AllowManageDevices))]
   public override async Task<CommonResponse> CheckSystemNameAvailability(SystemNameAvailabilityRequest request, ServerCallContext context)
   {
      var validateResult = await _deviceRegistrationService.ValidateSystemNameAsync(request.SystemName);
      return new() { Error = string.Join("; ", validateResult.Errors) };
   }

   [Authorize(nameof(StandardPermissionProvider.AllowManageDevices))]
   public override async Task<Empty> DeleteShared(DeviceProto request, ServerCallContext context)
   {
      // security
      var user = await _workContext.GetCurrentUserAsync();
      var unmappingDevice = (await _deviceService.GetSharedDevicesAsync(new() { UserId = user.Id, DeviceId = request.Id })).FirstOrDefault();

      if (unmappingDevice is not null)
         await _deviceService.UnshareDeviceAsync(unmappingDevice.Id, user.Id);

      return new Empty();
   }

   [Authorize(nameof(StandardPermissionProvider.AllowManageDevices))]
   public override Task<Empty> UpdateShared(DeviceProto request, ServerCallContext context)
   {
      return base.UpdateShared(request, context);
   }


   [Authorize(nameof(StandardPermissionProvider.AllowManageDevices))]
   public override async Task<Empty> ShareDevice(ShareRequest request, ServerCallContext context)
   {
      var subject = await _workContext.GetCurrentUserAsync();

      if (!await _userService.IsAdminAsync(subject) && !await _deviceService.IsUserDeviceAsync(subject.Id, request.EntityId))
         throw new RpcException(new(StatusCode.PermissionDenied, "You cannot share not your own device"));

      var user = _userSettings.UsernamesEnabled 
         ? await _userService.GetUserByUsernameAsync(request.UserName) 
         : await _userService.GetUserByEmailAsync(request.UserName);

      await _deviceService.ShareDeviceAsync(user.Id, request.EntityId);

      return new();
   }


   [Authorize(nameof(StandardPermissionProvider.AllowManageDevices))]
   public override async Task<Empty> UnshareDevice(ShareRequest request, ServerCallContext context)
   {
      var subject = await _workContext.GetCurrentUserAsync();

      if (!await _userService.IsAdminAsync(subject) && !await _deviceService.IsUserDeviceAsync(subject.Id, request.EntityId))
         throw new RpcException(new(StatusCode.PermissionDenied, "You cannot stop sharing not your own device"));

      var user = _userSettings.UsernamesEnabled
         ? await _userService.GetUserByUsernameAsync(request.UserName)
         : await _userService.GetUserByEmailAsync(request.UserName);

      if (user is not null)
         await _deviceService.UnshareDeviceAsync(user.Id, request.EntityId);

      return new();
   }
   #endregion
}
