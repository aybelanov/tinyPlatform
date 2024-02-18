using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hub.Core;
using Hub.Core.Domain.Media;
using Hub.Core.Domain.Users;
using Hub.Data.Extensions;
using Hub.Services.Clients;
using Hub.Services.Clients.Devices;
using Hub.Services.Common;
using Hub.Services.Devices;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Media;
using Hub.Services.Users;
using Hub.Web.Framework.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Shared.Clients;
using Shared.Clients.Configuration;
using Shared.Clients.Proto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auto = Hub.Core.Infrastructure.Mapper.AutoMapperConfiguration;

namespace Hub.Web.Grpc.Clients;

[EnableCors(PolicyName = WebFrameworkDefaults.CorsPolicyName)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserDefaults.TelemetryRoles)]
public class CommonGrpcService : CommonRpc.CommonRpcBase
{
   #region fields

   private readonly IDeviceRegistrationService _deviceRegistrationService;
   private readonly IGenericAttributeService _gaService;
   private readonly IWorkContext _workContext;
   private readonly IPictureService _pictureService;
   private readonly UserSettings _userSettings;
   private readonly ICommunicator _communicator;
   private readonly IDeviceActivityService _deviceActivityService;
   private readonly IUserActivityService _userActivityService;
   private readonly IUserService _userService;
   private readonly IDeviceService _deviceService;
   private readonly ILocalizationService _localizationService;

   #endregion

   #region Ctor

   public CommonGrpcService(IDeviceRegistrationService deviceRegistrationService,
      IGenericAttributeService gaService,
      IWorkContext workContext,
      IDeviceActivityService deviceActivityService,
      IUserActivityService userActivityService,
      IUserService userService,
      ILocalizationService localizationService,
      IDeviceService deviceService,
      IPictureService pictureService,
      UserSettings userSettings, 
      ICommunicator communicator)
   {
      _deviceRegistrationService = deviceRegistrationService;
      _workContext = workContext;
      _pictureService = pictureService;
      _userSettings = userSettings;
      _communicator = communicator;
      _gaService = gaService;
      _userService = userService;
      _deviceActivityService = deviceActivityService;
      _userActivityService = userActivityService;
      _deviceService = deviceService;
      _localizationService = localizationService;
   }

   #endregion

   #region Methods


   [Authorize(Roles = UserDefaults.TelemetryRoles)]
   public override async Task<UserProtos> GetUsers(FilterProto request, ServerCallContext context)
   {
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      var user = await _workContext.GetCurrentUserAsync();

      var isAdmin = await _userService.IsAdminAsync(user);

      if (!isAdmin && !(request.MonitorId > 0 || request.DeviceId > 0))
         throw new RpcException(new(StatusCode.PermissionDenied, "You cannot get not your users"));

      if(!isAdmin)
         filter.UserId = user.Id;   

      var users = await _userService.GetUsersByFilterAsync(filter);

      var userProtos = new UserProtos() { TotalCount = users.TotalCount };
      var protos = await users.SelectAwait(async user =>
      {
         var proto = Auto.Mapper.Map<UserProto>(user);
         proto.AvatarUrl = await _pictureService.GetPictureUrlAsync(user.AvatarPictureId, 0, _userSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);

         return proto;  
         
      }).ToListAsync();

      userProtos.Users.AddRange(protos);
      return userProtos;   
   }


   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public override async Task<UserSelectItemProtos> GetUserSelectItems(FilterProto request, ServerCallContext context)
   {
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      var users = await _userService.GetUserSelectItemsByFilterAsync(filter);

      var userProtos = new UserSelectItemProtos();
      userProtos.TotalCount = users.TotalCount;

      var protos = await users.SelectAwait(async user =>
      {
         var proto = new UserSelectItemProto
         {
            Id = user.Id,
            Username = user.Username,
            AvatarUrl = await _pictureService.GetPictureUrlAsync(user.AvatarPictureId, 0, _userSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar)
         };

         return proto;

      }).ToListAsync();

      userProtos.Users.AddRange(protos);
      return userProtos;
   }

   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public override async Task<ActivityLogRecordProtos> GetUserActivityLog(FilterProto request, ServerCallContext context)
   {
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      var records = await _userActivityService.GetActivitiesByDynamicFilterAsync(filter);
      var protos = new ActivityLogRecordProtos();
      protos.Records.AddRange(Auto.Mapper.Map<List<ActivityLogRecordProto>>(records));
      protos.TotalCount = records.TotalCount;

      return protos;
   }

   [Authorize(Roles = UserDefaults.TelemetryAdminRoles)]
   public override async Task<ActivityLogRecordProtos> GetDeviceActivityLog(FilterProto request, ServerCallContext context)
   {
      if (!request.DeviceId.HasValue || request.DeviceId.Value < 1)
         throw new RpcException(new(StatusCode.InvalidArgument, "Device id has not set."));

      var user = await _workContext.GetCurrentUserAsync();
      var filter = Auto.Mapper.Map<DynamicFilter>(request);

      // security
      if (!await _userService.IsAdminAsync(user))
        _ = (await _deviceService.GetOwnDevicesAsync(new() { UserId = user.Id, DeviceId = filter.DeviceId })).FirstOrDefault()
            ?? throw new RpcException(new(StatusCode.PermissionDenied, "It's not your device."));

      var records = await _deviceActivityService.GetActivitiesByDynamicFilterAsync(filter);
      var protos = new ActivityLogRecordProtos();
      protos.Records.AddRange(Auto.Mapper.Map<List<ActivityLogRecordProto>>(records));
      protos.TotalCount = records.TotalCount;

      return protos;
   }

   [Authorize(Roles = UserDefaults.TelemetryAdminRoles)]
   public override async Task<CommonResponse> CheckUserNameAvalability(SystemNameAvailabilityRequest request, ServerCallContext context)
   {
      if (string.IsNullOrEmpty(request.SystemName))
      {
         var error = await _localizationService.GetResourceAsync("Account.Register.Errors.UsernameIsNotProvided");
         return new CommonResponse() { Error = error };
      }

      var user = _userSettings.UsernamesEnabled ? await _userService.GetUserByUsernameAsync(request.SystemName) : await _userService.GetUserBySystemNameAsync(request.SystemName);
      if (user is null || user.IsDeleted || !user.IsActive) 
      {
         var error = await _localizationService.GetResourceAsync("Account.Register.Errors.UserNotExists");
         return new CommonResponse() { Error = error };
      }

      return new();
   }


   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public override Task<Empty> StartImpersonate(UserProto request, ServerCallContext context)
   {
      return base.StartImpersonate(request, context);
   }

   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public override Task<Empty> StopImpersonate(Empty request, ServerCallContext context)
   {
      return base.StopImpersonate(request, context);
   }

   #endregion
}
