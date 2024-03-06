using Hub.Core.Configuration;
using Hub.Core.Infrastructure;
using Hub.Web.Grpc.Clients;
using Hub.Web.Grpc.Devices;
using Hub.Web.Grpc.Interceptors;
using Hub.Web.Hubs;
using Hub.Web.Hubs.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Hub.Web.Infrastructure;

/// <summary>
/// Represents extensions of IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
   /// <summary>
   /// Adds SignalR services
   /// </summary>
   /// <param name="services">Service collection</param>
   public static void AddAppSignalR(this IServiceCollection services)
   {
      var commonConfig = Singleton<AppSettings>.Instance.Get<CommonConfig>();
      services.AddSignalR(o => o.EnableDetailedErrors = commonConfig.DisplayFullErrorStack)
      .AddMessagePackProtocol()
      .AddHubOptions<DashboardHub>(o =>
      {
         o.AddFilter<RegisterConnectionFilter>();
         o.AddFilter<SubscribeGroupsFilter>();
      })
      .AddHubOptions<DeviceHub>(o => o.AddFilter<SubscribeGroupsFilter>());

      services.AddResponseCompression(opts => opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" }));
   }

   /// <summary>
   /// Adds Grpc services
   /// </summary>
   /// <param name="services">Service collection</param>
   public static void AddAppGrpc(this IServiceCollection services)
   {
      var commonConfig = Singleton<AppSettings>.Instance.Get<CommonConfig>();
      services.AddGrpc(c => c.EnableDetailedErrors = commonConfig.DisplayFullErrorStack)
      .AddServiceOptions<CommonGrpcService>(c => c.Interceptors.Add<ConnectionIdInterceptor>())
      .AddServiceOptions<DeviceGrpcService>(c => c.Interceptors.Add<ConnectionIdInterceptor>())
      .AddServiceOptions<DownloadTaskGrpcService>(c => c.Interceptors.Add<ConnectionIdInterceptor>())
      .AddServiceOptions<MonitorGrpcService>(c => c.Interceptors.Add<ConnectionIdInterceptor>())
      .AddServiceOptions<SensorGrpcService>(c => c.Interceptors.Add<ConnectionIdInterceptor>())
      .AddServiceOptions<SensorRecordGrpcService>(c => c.Interceptors.Add<ConnectionIdInterceptor>())
      .AddServiceOptions<WidgetGrpcService>(c => c.Interceptors.Add<ConnectionIdInterceptor>())
      .AddServiceOptions<PresentationGrpcService>(c => c.Interceptors.Add<ConnectionIdInterceptor>())
      .AddServiceOptions<DispatcherGrpcService>(c => c.Interceptors.Add<CheckConfigurationInterceptor>());
   }
}