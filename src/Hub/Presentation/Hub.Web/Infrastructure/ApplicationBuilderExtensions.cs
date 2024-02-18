using Hub.Services.Clients;
using Hub.Web.Grpc.Clients;
using Hub.Web.Grpc.Devices;
using Hub.Web.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;

namespace Hub.Web.Infrastructure;

/// <summary>
/// Represents extensions of IApplicationBuilder
/// </summary>
public static class ApplicationBuilderExtensions
{
   /// <summary>
   /// Application WebAPi endponts configuration
   /// </summary>
   /// <param name="application">Application builder</param>
   public static void UseApiEndpoinst(this IApplicationBuilder application)
   {   
      // https://docs.microsoft.com/ru-ru/aspnet/core/blazor/tutorials/signalr-blazor?view=aspnetcore-6.0&tabs=visual-studio&pivots=webassembly
      application.UseEndpoints(endpoints =>
      {
         // Webapi
         endpoints.MapControllers();
      });
   }

   /// <summary>
   /// Application Grpc endponts configuration
   /// </summary>
   /// <param name="application">Application builder</param>
   public static void UseGrpcEndpoinst(this IApplicationBuilder application)
   {
      application.UseEndpoints(endpoints =>
      {
         endpoints.MapGrpcService<DispatcherGrpcService>();
         endpoints.MapGrpcService<CommonGrpcService>().EnableGrpcWeb();
         endpoints.MapGrpcService<DeviceGrpcService>().EnableGrpcWeb();
         endpoints.MapGrpcService<MonitorGrpcService>().EnableGrpcWeb();
         endpoints.MapGrpcService<SensorGrpcService>().EnableGrpcWeb();
         endpoints.MapGrpcService<WidgetGrpcService>().EnableGrpcWeb();
         endpoints.MapGrpcService<SensorRecordGrpcService>().EnableGrpcWeb();
         endpoints.MapGrpcService<DownloadTaskGrpcService>().EnableGrpcWeb();
         endpoints.MapGrpcService<PresentationGrpcService>().EnableGrpcWeb();
      });
   }

   /// <summary>
   /// Application SignalR endponts configuration
   /// </summary>
   /// <param name="application">Application builder</param>
   public static void UseSignalrEndpoinst(this IApplicationBuilder application)
   {
      application.UseEndpoints(endpoints =>
      {
         endpoints.MapHub<DashboardHub>(ClientDefaults.SignalrEndpoint, o => o.Transports = HttpTransportType.WebSockets);
      });
   }
}
