using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hub.Core;
using Hub.Services.Clients.Records;
using Hub.Services.Security;
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
public class SensorRecordGrpcService(IUserService userService,
   ISensorRecordService sensorRecordService,
   IWorkContext workContext) : SensorRecordRpc.SensorRecordRpcBase
{
   #region Methods

   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public override async Task<Empty> Delete(FilterProto request, ServerCallContext context)
   {
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      await sensorRecordService.DeleteRecordsByFilterAsync(filter);
      return new Empty();
   }

   [Authorize(nameof(StandardPermissionProvider.AllowGetData))]
   public override async Task<SensorRecordProtos> GetRecords(FilterProto request, ServerCallContext context)
   {
      var user = await workContext.GetCurrentUserAsync();
      var filter = Auto.Mapper.Map<DynamicFilter>(request);

      // only admins can get not their own sensor records
      if (await userService.IsAdminAsync(user))
         filter.UserId ??= user.Id;
      else
         filter.UserId = user.Id;

      var records = await sensorRecordService.GetRecordsAsync(filter);
      var recordsProto = new SensorRecordProtos();
      var protos = Auto.Mapper.Map<List<SensorRecordProto>>(records);
      recordsProto.Records.AddRange(protos);
      recordsProto.TotalCount = records.TotalCount;

      return recordsProto;
   }

   [Authorize(nameof(StandardPermissionProvider.AllowGetData))]
   public override async Task<ChartSeriesProto> GetChartData(ChartRequestProto request, ServerCallContext context)
   {
      // TODO check security per user
      var chartRequest = Auto.Mapper.Map<ChartRequest>(request);
      // only ten series should be allowed
      chartRequest.SensorIds = chartRequest.SensorIds.Take(10);
      var charts = await sensorRecordService.GetChartSeriesAsync(chartRequest);

      var seriesProto = new ChartSeriesProto();
      foreach (var chart in charts)
      {
         var setProto = new ChartSetProto();
         var pointsProto = Auto.Mapper.Map<List<ChartPointProto>>(chart.Data);
         setProto.EntityId = chart.EntityId;
         setProto.AbscissaName = chart.AbscissaName;
         setProto.Name = chart.Name;
         setProto.OrdinateName = chart.OrdinateName;
         setProto.Data.AddRange(pointsProto);

         seriesProto.Series.Add(setProto);
      }

      return seriesProto;
   }


   [Authorize(Roles = UserDefaults.AdministratorsRoleName)]
   public override async Task<DataStatisticsProtos> GetAllDataStatistics(FilterProto request, ServerCallContext context)
   {
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      var data = await sensorRecordService.GetDataStatisticsAsync(filter);

      var reply = new DataStatisticsProtos();
      var protos = Auto.Mapper.Map<List<DataStatisticsProto>>(data);
      reply.Data.AddRange(protos);

      return reply;
   }

   [Authorize(nameof(StandardPermissionProvider.AllowGetData))]
   public override async Task<DataStatisticsProtos> GetUserDataStatistics(FilterProto request, ServerCallContext context)
   {
      var user = await workContext.GetCurrentUserAsync();
      request.UserId = user.Id;

      return await GetAllDataStatistics(request, context);  
   }

   [Authorize(nameof(StandardPermissionProvider.AllowGetData))]
   public override async Task<RawGeoDataProtos> GetTrack(FilterProto request, ServerCallContext context)
   {
      var user = await workContext.GetCurrentUserAsync();
      if (!await userService.IsAdminAsync(user))
         request.UserId = user.Id;
      
      var filter = Auto.Mapper.Map<DynamicFilter>(request);
      var track = await sensorRecordService.GetTrackAsync(filter);

      var reply = new RawGeoDataProtos();
      reply.Data = track;//.AddRange(Auto.Mapper.Map<List<RawGeoDataProto>>(track));

      return reply;
   }


   [Authorize(nameof(StandardPermissionProvider.AllowGetData))]
   public override async Task<VideoSegmentProtos> GetVideoSegments(FilterProto request, ServerCallContext context)
   {
      var user = await workContext.GetCurrentUserAsync();
      if (!await userService.IsAdminAsync(user))
         request.UserId = user.Id;

      var filter = Auto.Mapper.Map<DynamicFilter>(request);

      var segments = await sensorRecordService.GetSegmentsByFilterAsync(filter);
      var protos = Auto.Mapper.Map<List<VideoSegmentProto>>(segments);
      var reply = new VideoSegmentProtos();
      reply.Segments.Add(protos);

      return reply;
   }

   #endregion
}