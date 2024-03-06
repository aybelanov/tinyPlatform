using AutoMapper;
using Clients.Dash.Domain;
using Clients.Dash.Pages.Configuration.Devices;
using Clients.Dash.Pages.Configuration.Monitors;
using Clients.Dash.Pages.Configuration.Sensors;
using Clients.Dash.Pages.Configuration.Widgets;
using Clients.Dash.Pages.Monitors;
using Clients.Widgets;
using Google.Protobuf.Collections;
using Radzen;
using Shared.Clients;
using Shared.Clients.Proto;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using static Clients.Dash.Pages.Configuration.Users.UserActivityTable;
using static Clients.Dash.Pages.Configuration.Users.UserTable;
using static Clients.Dash.Pages.Reports.Charts.ChartRequest;
using static Clients.Dash.Pages.Reports.Export.DownloadTaskRequest;
using static Clients.Dash.Pages.Reports.Export.DownloadTaskTable;
using static Clients.Widgets.Core.OpenLayerBase;
using static Clients.Widgets.VideoPlayer;
using Monitor = Clients.Dash.Domain.Monitor;

namespace Clients.Dash.Infrastructure.AutoMapper;

/// <summary>
/// Represents a automapper configuration class
/// </summary>
public class ModelMapperConfiguration : Profile
{
   /// <summary>
   /// Default Ctor
   /// </summary>
   public ModelMapperConfiguration()
   {
      #region Devices

      CreateMap<Device, DeviceProto>()
        .ForMember(dest => dest.LastActivityDate, opt => opt.Ignore());
      CreateMap<DeviceProto, Device>()
         .ForMember(dest => dest.LastActivityOnUtc, opt => opt.MapFrom(source => source.LastActivityDate.ToDateTimeFromUinxEpoch()));

      CreateMap<Device, DeviceModel>()
          .ForMember(x => x.Configuration, op => op.MapFrom(entity => ClientHelper.NormalizeJsonString(entity.Configuration, true)));

      CreateMap<DeviceModel, Device>()
          .ForMember(x => x.Configuration, op => op.MapFrom(model => ClientHelper.NormalizeJsonString(model.Configuration, false)));

      CreateMap<DeviceModel, DeviceProto>()
       .ForMember(dest => dest.LastActivityDate, opt => opt.Ignore())
       .ForMember(x => x.Configuration, op => op.MapFrom(model => ClientHelper.NormalizeJsonString(model.Configuration, false)));

      CreateMap<DeviceProto, DeviceModel>()
         .ForMember(dest => dest.LastActivityOnUtc, opt => opt.MapFrom(source => source.LastActivityDate.ToDateTimeFromUinxEpoch()))
         .ForMember(x => x.Configuration, op => op.MapFrom(proto => ClientHelper.NormalizeJsonString(proto.Configuration, true)));

      CreateMap<DeviceSelectItemProto, DeviceSelectItem>();

      CreateMap<DeviceMapItemProto, DeviceMapItem>();
      CreateMap<DeviceMapItem, Marker>()
         .ForMember(dest => dest.EntityId, op => op.MapFrom(source => source.Id));

      #endregion

      #region Sensors

      CreateMap<Sensor, SensorProto>();
      CreateMap<SensorProto, Sensor>();
      CreateMap<SensorSelectItemProto, SensorSelectItem>();
      CreateMap<SensorModel, SensorProto>()
          .ForMember(x => x.Configuration, op => op.MapFrom(model => ClientHelper.NormalizeJsonString(model.Configuration, false)));

      CreateMap<SensorProto, SensorModel>()
          .ForMember(x => x.Configuration, op => op.MapFrom(proto => ClientHelper.NormalizeJsonString(proto.Configuration, true)));


      CreateMap<Sensor, SensorModel>()
          .ForMember(x => x.Configuration, op => op.MapFrom(entity => ClientHelper.NormalizeJsonString(entity.Configuration, true)));

      CreateMap<SensorModel, Sensor>()
          .ForMember(x => x.Configuration, op => op.MapFrom(model => ClientHelper.NormalizeJsonString(model.Configuration, false)));

      #endregion

      #region Monitors

      CreateMap<Monitor, MonitorProto>();
      CreateMap<MonitorProto, Monitor>();
      CreateMap<Monitor, MonitorModel>();
      CreateMap<MonitorModel, Monitor>();
      CreateMap<MonitorProto, MonitorModel>();
      CreateMap<MonitorModel, MonitorProto>()
         .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(_ => string.Empty));

      CreateMap<MonitorViewProto, MonitorView>();
      CreateMap<MonitorView, MonitorViewModel>();
      CreateMap<PresentationViewProto, PresentationView>();
      CreateMap<PresentationView, PresentationViewModel>();

      CreateMap<Presentation, PresentationProto>();
      CreateMap<PresentationProto, Presentation>();
      CreateMap<Presentation, PresentationModel>();
      CreateMap<PresentationModel, Presentation>();
      CreateMap<PresentationProto, PresentationModel>();
      CreateMap<PresentationModel, PresentationProto>();

      #endregion

      #region Users

      CreateMap<User, UserModel>();
      CreateMap<UserProto, User>()
        .ForMember(dest => dest.LastActivityUtc, opt => opt.MapFrom(source => source.LastActivityUtc.ToDateTimeFromUinxEpoch()));

      CreateMap<UserSelectItemProto, UserSelectItem>();

      #endregion

      #region Activity log

      CreateMap<ActivityLogRecord, ActivityLogRecordModel>();
      CreateMap<ActivityLogRecordProto, ActivityLogRecord>()
         .ForMember(dest => dest.CreatedOnUtc, opt => opt.MapFrom(source => source.CreatedOnUtc.ToDateTimeFromUinxEpoch()));

      #endregion

      #region Widgets

      CreateMap<Widget, WidgetModel>();
      CreateMap<WidgetModel, Widget>();

      CreateMap<Widget, WidgetProto>()
         .ForMember(x => x.Adjustment, op => op.MapFrom(src => ClientHelper.NormalizeJsonString(src.Adjustment, false)));

      CreateMap<WidgetModel, WidgetProto>()
         .ForMember(x => x.Adjustment, op => op.MapFrom(src => ClientHelper.NormalizeJsonString(src.Adjustment, false)));

      CreateMap<WidgetProto, WidgetModel>()
         .ForMember(x => x.Adjustment, op => op.MapFrom(src => ClientHelper.NormalizeJsonString(src.Adjustment, true)));

      CreateMap<WidgetProto, Widget>()
         .ForMember(x => x.Adjustment, op => op.MapFrom(src => ClientHelper.NormalizeJsonString(src.Adjustment, true)));

      CreateMap<WidgetSelectItemProto, WidgetSelectItem>();
      CreateMap<PresentationSelectItemProto, PresentationSelectItem>();
      CreateMap<PresentationSelectItem, PresentationSelectItemProto>();
      #endregion

      #region Sensor records

      CreateMap<SensorRecordProto, SensorRecord>();
      //CreateMap<SensorRecordModel, SensorRecordProto>()
      //    .ForMember(destination => destination.Bytes, options => options.MapFrom(src => ByteString.CopyFrom(src.Bytes ?? Array.Empty<byte>())));

      #endregion

      #region Charts

      CreateMap<ChartRequestModel, ChartRequest>();
      CreateMap<ChartRequest, ChartRequestProto>()
         .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From.ToUnixEpochTime()))
         .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To.ToUnixEpochTime()));

      CreateMap<ChartPointProto, ChartPoint>();
      CreateMap<ChartSetProto, ChartSet>();

      #endregion

      #region Video

      CreateMap<VideoSegmentProto, Segment>()
         .ForMember(dest => dest.OnCreatedUtc, opt => opt.MapFrom(src => src.OnCreatedUtc.ToDateTimeFromUinxEpoch()));

      #endregion

      #region Download task

      CreateMap<DownloadTask, DownloadTaskModel>();
      CreateMap<DownloadTaskModel, DownloadTask>();
      CreateMap<DownloadRequestModel, DownloadRequest>();

      CreateMap<DownloadTaskProto, DownloadTask>()
         .ForMember(dest => dest.TaskDateTimeUtc, opt => opt.MapFrom(src => src.TaskDateTimeUtc.ToDateTimeFromUinxEpoch()))
         .ForMember(dest => dest.ReadyDateTimeUtc, opt => opt.MapFrom(src => src.ReadyDateTimeUtc.ToDateTimeFromUinxEpoch()));

      CreateMap<DownloadRequest, DownloadRequestProto>()
         .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To.ToUnixEpochTime()))
         .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From.ToUnixEpochTime()));

      #endregion

      #region DataStatistics

      CreateMap<DataStatisticsProto, TimelineChart.Point>();

      #endregion

      #region Common

      CreateMap<LoadDataArgs, DynamicFilter>();
      CreateMap<DynamicFilter, FilterProto>()
         .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To.ToUnixEpochTime()))
         .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From.ToUnixEpochTime()))
         .ForMember(dest => dest.TimeSpan, opt => opt.MapFrom(src => src.TimeSpan.ToNullableTicks()))
         .ForMember(dest => dest.ConnectionStatuses, opt => opt.MapFrom(source => source.ConnectionStatuses.Select(x => (int)x)))
         .ForMember(dest => dest.Ids, opt => opt.MapFrom(src => CopyIds(src.Ids)));

      CreateMap<LoadDataArgs, FilterProto>();

      #endregion
   }

   #region Utilites

   private static RepeatedField<long> CopyIds(IEnumerable<long> ids)
   {
      var res = new RepeatedField<long>();
      res.AddRange(ids ?? Array.Empty<long>());
      return res;
   }

   #endregion
}