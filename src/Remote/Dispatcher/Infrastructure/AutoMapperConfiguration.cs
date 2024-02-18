using AutoMapper;

namespace Devices.Dispatcher.Infrastructure;

/// <summary>
/// AutoMapper configuration
/// </summary>
public static class AutoMapperConfiguration
{
   /// <summary>
   /// Mapper
   /// </summary>
   public static IMapper Mapper { get; private set; }

   /// <summary>
   /// Mapper configuration
   /// </summary>
   public static MapperConfiguration MapperConfiguration { get; private set; }

   /// <summary>
   /// Initialize mapper
   /// </summary>
   /// <param name="config">Mapper configuration</param>
   public static IMapper Init(MapperConfiguration config)
   {
      MapperConfiguration = config;
      Mapper = config.CreateMapper();

      return Mapper;
   }
}