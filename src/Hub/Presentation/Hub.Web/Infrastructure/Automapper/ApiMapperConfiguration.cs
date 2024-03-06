using AutoMapper;
using Google.Protobuf;
using Hub.Core.Domain.Clients;
using Hub.Core.Domain.Logging;
using Hub.Core.Domain.Users;
using Hub.Core.Infrastructure.Mapper;
using Hub.Services.Clients.Devices;
using Hub.Services.Clients.Monitors;
using Hub.Services.Clients.Reports;
using Hub.Services.Clients.Sensors;
using Hub.Services.Clients.Widgets;
using Shared.Clients;
using Shared.Clients.Proto;
using Shared.Common;
using System;
using System.Linq;

namespace Hub.Web.Infrastructure.Automapper;

/// <summary>
/// AutoMapper configuration for grpc, signalr and DTO
/// </summary>
public class ApiMapperConfiguration : Profile, IOrderedMapperProfile
{
   /// <summary>
   /// Ctor
   /// </summary>
   public ApiMapperConfiguration()
   {
      CreateMap<FilterProto, DynamicFilter>()
         .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To.ToDateTimeFromUinxEpoch()))
         .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From.ToDateTimeFromUinxEpoch()))
         .ForMember(dest => dest.TimeSpan, opt => opt.MapFrom(src => src.TimeSpan.ToNullableTimeSpan()))
         .ForMember(dest => dest.ConnectionStatuses, opt => opt.MapFrom(source => source.ConnectionStatuses.Select(x => (OnlineStatus)x)));

      CreateMap<User, UserProto>()
         .ForMember(dest => dest.LastActivityUtc, opt => opt.MapFrom(source => source.LastActivityUtc.ToUnixEpochTime()));

      CreateMap<ActivityLog, ActivityLogRecordProto>().ForMember(dest => dest.CreatedOnUtc, opt => opt.MapFrom(source => source.CreatedOnUtc.ToUnixEpochTime()));

      CreateMap<Monitor, MonitorProto>();
      CreateMap<MonitorProto, Monitor>();

      //CreateMap<MonitorView, MonitorViewProto>();
      CreateMap<Monitor, MonitorViewProto>();
      //CreateMap<PresentationView, PresentationViewProto>();
      CreateMap<Presentation, PresentationViewProto>();

      CreateMap<Widget, WidgetProto>();
      CreateMap<WidgetProto, Widget>();

      CreateMap<WidgetSelectItem, WidgetSelectItemProto>();
      CreateMap<PresentationSelectItem, PresentationSelectItemProto>();
      CreateMap<PresentationSelectItemProto, PresentationSelectItem>();

      CreateMap<Presentation, PresentationProto>();
      CreateMap<PresentationProto, Presentation>();

      CreateMap<Device, Shared.Clients.Proto.DeviceProto>()
         .ForMember(dest => dest.Password, opt => opt.Ignore())
         .ForMember(dest => dest.LastActivityDate, opt => opt.MapFrom(source => source.LastActivityOnUtc.ToUnixEpochTime()));

      CreateMap<Shared.Clients.Proto.DeviceProto, Device>()
         .ForMember(dest => dest.CreatedOnUtc, opt => opt.Ignore())
         .ForMember(dest => dest.IsActive, opt => opt.Ignore())
         .ForMember(dest => dest.CannotLoginUntilDateUtc, opt => opt.Ignore())
         .ForMember(dest => dest.FailedLoginAttempts, opt => opt.Ignore())
         .ForMember(dest => dest.Guid, opt => opt.Ignore())
         .ForMember(dest => dest.LastIpAddress, opt => opt.Ignore())
         .ForMember(dest => dest.UpdatedOnUtc, opt => opt.Ignore())
         .ForMember(dest => dest.PictureId, opt => opt.Ignore())
         .ForMember(dest => dest.AdminComment, opt => opt.Ignore())
         .ForMember(dest => dest.OwnerName, opt => opt.Ignore())
         .ForMember(dest => dest.ConnectionStatus, opt => opt.Ignore())
         .ForMember(dest => dest.LastActivityOnUtc, opt => opt.Ignore());

      CreateMap<DeviceSelectItem, DeviceSelectItemProto>();
      CreateMap<DeviceMapItem, DeviceMapItemProto>();

      CreateMap<Sensor, Shared.Clients.Proto.SensorProto>();
      CreateMap<Shared.Clients.Proto.SensorProto, Sensor>();
      CreateMap<SensorSelectItem, SensorSelectItemProto>();

      CreateMap<Device, Shared.Devices.Proto.DeviceProto>();
      CreateMap<Sensor, Shared.Devices.Proto.SensorProto>();
      CreateMap<Shared.Devices.Proto.SensorRecordProto, SensorRecord>();
      CreateMap<SensorRecord, Shared.Clients.Proto.SensorRecordProto>()
         .ForMember(destination => destination.Bytes, options => options.MapFrom(src => ByteString.CopyFrom(src.Bytes ?? Array.Empty<byte>())));

      CreateMap<DownloadTask, DownloadTaskProto>()
         .ForMember(dest => dest.TaskDateTimeUtc, opt => opt.MapFrom(src => src.TaskDateTimeUtc.ToUnixEpochTime()))
         .ForMember(dest => dest.ReadyDateTimeUtc, opt => opt.MapFrom(src => src.ReadyDateTimeUtc.ToUnixEpochTime()));

      CreateMap<DownloadTaskProto, DownloadTask>()
         .ForMember(dest => dest.TaskDateTimeUtc, opt => opt.MapFrom(src => src.TaskDateTimeUtc.ToDateTimeFromUinxEpoch()))
         .ForMember(dest => dest.ReadyDateTimeUtc, opt => opt.MapFrom(src => src.ReadyDateTimeUtc.ToDateTimeFromUinxEpoch()));

      CreateMap<DownloadRequestProto, DownloadRequest>()
         .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To.ToDateTimeFromUinxEpoch()))
         .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From.ToDateTimeFromUinxEpoch()));

      CreateMap<ChartRequestProto, ChartRequest>()
         .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From.ToDateTimeFromUinxEpoch()))
         .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To.ToDateTimeFromUinxEpoch()));
      CreateMap<ChartSet, ChartSetProto>();
      CreateMap<ChartPoint, ChartPointProto>();

      CreateMap<DataStaticticsItem, DataStatisticsProto>()
         .ForMember(dest => dest.Value, opt => opt.MapFrom(source => source.RecordCount));

      CreateMap<VideoSegment, VideoSegmentProto>()
        .ForMember(dest => dest.OnCreatedUtc, opt => opt.MapFrom(src => src.OnCreatedUtc.ToUnixEpochTime()))
        .ForMember(destination => destination.SegmentName, options => options.MapFrom(src => UnsafeByteOperations.UnsafeWrap(src.Guid.ToByteArray() ?? Array.Empty<byte>())));
   }

   /// <summary>
   /// Order of this mapper implementation
   /// </summary>
   public int Order => 0;
}
