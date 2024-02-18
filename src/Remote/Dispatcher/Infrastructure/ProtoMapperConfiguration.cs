using AutoMapper;
using Devices.Dispatcher.Configuration;
using Devices.Dispatcher.Domain;
using Google.Protobuf;
using Shared.Devices.Proto;
using System;
using System.Linq;

namespace Devices.Dispatcher.Infrastructure;

#pragma warning disable CS1591

public class ProtoMapperConfiguration : Profile
{
   public ProtoMapperConfiguration()
   {
      #region Domain <--> Proto

      CreateMap<DeviceSettings, DeviceProto>();
      CreateMap<DeviceProto, DeviceSettings>();

      CreateMap<HubConnections, DeviceProto>();
      CreateMap<DeviceProto, HubConnections>();

      CreateMap<Sensor, SensorProto>();
      CreateMap<SensorProto, Sensor>();

      CreateMap<SensoRecord, SensorRecordProto>()
          .ForMember(destination => destination.Bytes, options => options.MapFrom(src => ByteString.CopyFrom(src.Bytes ?? Array.Empty<byte>())))
          .ForMember(destination => destination.Id, options => options.Ignore());

      CreateMap<SensorRecordProto, SensoRecord>()
          .ForMember(destination => destination.Bytes, options => options.MapFrom(src => src.Bytes.ToArray() ?? new byte[] { }));

      #endregion
   }
}
