using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hub.Core;
using Hub.Core.Domain.Clients;
using Hub.Data;
using Hub.Services.Clients;
using Hub.Services.Clients.Records;
using Hub.Services.Common;
using Hub.Services.Devices;
using Hub.Services.Logging;
using Hub.Services.Media;
using Hub.Services.Security;
using Hub.Web.Framework.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Clients.Configuration;
using Shared.Clients.SignalR;
using Shared.Devices.Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Auto = Hub.Core.Infrastructure.Mapper.AutoMapperConfiguration;

namespace Hub.Web.Grpc.Devices;

[EnableCors(PolicyName = WebFrameworkDefaults.CorsPolicyName)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserDefaults.DevicesRoleName)]
[EnableRateLimiting("device")]
public class DispatcherGrpcService : DeviceCalls.DeviceCallsBase
{
   #region fields

   private readonly ICommunicator _communicator;
   private readonly IDeviceActivityService _deviceActivityService;
   private readonly IHubSensorService _sensorService;
   private readonly IWorkContext _workContext;
   private readonly AppDbContext _dbContext;
   private readonly IVideoStreamService _videoStreamService;
   private readonly ISensorRecordService _sensorRecordService;
   private readonly ILogger _logger;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly IPermissionService _permissionService;
   private readonly IWebHelper _webHelper;
   private readonly IHubDeviceService _deviceService;
   private readonly IServiceScopeFactory _serviceScopeFactory;
   private readonly DeviceSettings _deviceSettings;

   #endregion

   #region Ctors

   public DispatcherGrpcService(
      ICommunicator communicator,
      IDeviceActivityService deviceActivityService,
      IPermissionService permissionService,
      IHubSensorService sensorService,
      IWebHelper webHelper,
      IHubDeviceService deviceService,
      ISensorRecordService sensorRecordService,
      IGenericAttributeService genericAttributeService,
      IWorkContext workContext,
      DeviceSettings deviceSettings,   
      AppDbContext dbContext,
      IVideoStreamService videoStreamService,
      ILogger logger,
      IServiceScopeFactory serviceScopeFactory)
   {
      _communicator = communicator;
      _deviceActivityService = deviceActivityService;
      _workContext = workContext;
      _dbContext = dbContext;
      _videoStreamService = videoStreamService;
      _logger = logger;
      _genericAttributeService = genericAttributeService;
      _sensorService = sensorService;
      _permissionService = permissionService;
      _webHelper = webHelper;
      _deviceService = deviceService;
      _deviceSettings = deviceSettings;
      _sensorRecordService = sensorRecordService;
      _serviceScopeFactory = serviceScopeFactory;
   }

   #endregion

   #region Methods

   [Authorize(nameof(StandardPermissionProvider.GetDeviceConfig))]
   public override async Task<DeviceProto> ConfigurationCall(Empty request, ServerCallContext context)
   {
      var device = await _workContext.GetCurrentDeviceAsync();
      var protoDevice = Auto.Mapper.Map<DeviceProto>(device);

      var sensors = (await _sensorService.GetSensorsByDeviceIdAsync(device.Id)).Where(x => x.Enabled);
      var protoSensors = Auto.Mapper.Map<List<SensorProto>>(sensors.ToList());
      protoDevice.Sensors.AddRange(protoSensors);

      protoDevice.ModifiedTicks = await _genericAttributeService.GetAttributeAsync<long>(device, ClientDefaults.DeviceConfigurationVersion);

      return protoDevice;
   }


   [Authorize(nameof(StandardPermissionProvider.SetupP2PChannel))]
   public override async Task Point2PointStream(IAsyncStreamReader<ClientMsg> requestStream, IServerStreamWriter<ServerMsg> responseStream, ServerCallContext context)
   {
      var device = await _workContext.GetCurrentDeviceAsync();
      await _communicator.RegisterDeviceChannelAsync(device.Id, context);
      await _deviceActivityService.InsertActivityAsync(device, "Device.Connect", $"Device \"{device.SystemName}\" has connected to Hub.");
      
      device.LastActivityOnUtc = DateTime.UtcNow;
      device.LastIpAddress = _webHelper.GetCurrentIpAddress();
      await _deviceService.UpdateDeviceAsync(device);

      await _communicator.DeviceStatusChangedAsync(device.Id, new DeviceConnectionStatus()
      {
         DeviceId = device.Id,
         IPAddress = _deviceSettings.StoreIpAddresses ? device.LastIpAddress : HubCommonDefaults.SpoofedIp
      });

      var readTask = Task.Run(async () =>
      {
         try
         {
            await foreach (var message in requestStream.ReadAllAsync(context.CancellationToken))
            {
               switch (message.ResultCase)
               {
                  case ClientMsg.ResultOneofCase.CommonResponse:
                     await _communicator.ClientsNotifyAsync(device, SignalRDefaults.DeviceNotificationMethod, message.CommonResponse.Notification);
                     break;
                  case ClientMsg.ResultOneofCase.CommandResponse:
                     await _communicator.ClientNotifyAsync(message.Receiver, SignalRDefaults.ClientMessageMethod, device.Id, message.CommandResponse.Notification);
                     break;
               }
            }
         }
         catch (Exception ex)
         {
            await _logger.ErrorAsync("P2P client side error", ex);
         }
         finally
         {
            await _communicator.UnregisterDeviceChannelAsync(device.Id);
         }
      });

      try
      {
         while (!readTask.IsCompleted)
         {
            var command = await _communicator.GetMessageForDeviceAsync(device.Id);
            var reply = command();
            if (reply != null)
            {
               await responseStream.WriteAsync(reply, context.CancellationToken);
            }
         }
      }
      catch (Exception ex)
      {
         await _logger.ErrorAsync("P2P server side error", ex);
      }
      finally
      {
         await _communicator.UnregisterDeviceChannelAsync(device.Id);
         
         await _deviceActivityService.InsertActivityAsync(device, "Device.Disconnect", $"Device \"{device.SystemName}\" has disconnected from Hub.");
       
         device.LastActivityOnUtc = DateTime.UtcNow;
         await _deviceService.UpdateDeviceAsync(device);

         await _communicator.DeviceStatusChangedAsync(device.Id, new DeviceConnectionStatus() { DeviceId = device.Id, IPAddress = null });
      }
   }


   [Authorize(nameof(StandardPermissionProvider.SaveSensorData))]
   public override async Task<Empty> SensorDataCall(SensorRecordProtos request, ServerCallContext context)
   {
      var device = await _workContext.GetCurrentDeviceAsync();
      var sensors = await _sensorService.GetSensorsByDeviceIdAsync(device.Id);
      var sensorIds = sensors.Where(x => x.Enabled).Select(x => x.Id);
      var now = DateTime.UtcNow; 

      // only records for registered and enabled sensors will be stored
      var allowedProtoRecords = request.Records.Where(x => sensorIds.Contains(x.SensorId));

      var records = Auto.Mapper.Map<List<SensorRecord>>(allowedProtoRecords);

      foreach (var record in records)
         record.CreatedTimeOnUtc = now;

      await _dbContext.BulkInsertAsync(records);

      await _communicator.SensorDataFlowAsync(records);

      return new Empty();
   }


   [Authorize(Policy = nameof(StandardPermissionProvider.SaveVideo))]
   public override async Task<Empty> VideoCall(IAsyncStreamReader<VideoSegmentProto> requestStream, ServerCallContext context)
   {
      var device = await _workContext.GetCurrentDeviceAsync();

      await foreach (var segment in requestStream.ReadAllAsync(context.CancellationToken))
      {
         var metaData = await _videoStreamService.GetFileMetadataAsync(segment.SegmentName);

         // security: save only segments from enabled sensors of the current authenticated device 
         if (metaData.Source.DeviceId == device.Id && metaData.Source.Enabled)
         {
            using var scope = _serviceScopeFactory.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var videoSegment = new VideoSegment()
            {
               IpcamId = metaData.Source.Id,
               OnCreatedUtc = new DateTime(segment.Timestamp, DateTimeKind.Utc),
               InboundName = segment.SegmentName,
               Extinf = segment.Duration,
               OnReceivedUtc = DateTime.UtcNow,
               Resolution = segment.Resolution,
            };

            await dbContext.AddAsync(videoSegment);
            await dbContext.SaveChangesAsync();
            
            await dbContext.AddAsync<VideoSegmentBinary>(new() { Binary = segment.Bytes.ToByteArray(), VideoSegmentId  = videoSegment.Id });
            await dbContext.SaveChangesAsync();

            // TODO save new segments to the disk. It will reduce db requests.

            await _communicator.SensorDataFlowAsync(videoSegment);
         }
      }

      return new Empty();
   }

   #endregion
}
