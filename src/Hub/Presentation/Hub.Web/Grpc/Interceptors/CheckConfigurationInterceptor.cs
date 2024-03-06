using Grpc.Core;
using Grpc.Core.Interceptors;
using Hub.Core;
using Hub.Services.Clients;
using Hub.Services.Common;
using Hub.Web.Grpc.Devices;
using Shared.Devices;
using System.Net;
using System.Threading.Tasks;

namespace Hub.Web.Grpc.Interceptors;

public class CheckConfigurationInterceptor(IWorkContext workContext, IGenericAttributeService genericAttributeService) : Interceptor
{
   private readonly IWorkContext _workContext = workContext;
   private readonly IGenericAttributeService _genericAttributeService = genericAttributeService;

   public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
      TRequest request,
      ServerCallContext context,
      UnaryServerMethod<TRequest,
      TResponse> continuation)
   {
      await EnsureConfigurationIsSync(context);
      return await continuation(request, context);
   }

   public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
      IAsyncStreamReader<TRequest> requestStream,
      IServerStreamWriter<TResponse> responseStream,
      ServerCallContext context,
      DuplexStreamingServerMethod<TRequest, TResponse> continuation)
   {
      await EnsureConfigurationIsSync(context);
      await continuation(requestStream, responseStream, context);
   }

   public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
      IAsyncStreamReader<TRequest> requestStream,
      ServerCallContext context,
      ClientStreamingServerMethod<TRequest, TResponse> continuation)
   {
      await EnsureConfigurationIsSync(context);
      return await continuation(requestStream, context);
   }


   #region Utils

   private async Task EnsureConfigurationIsSync(ServerCallContext context)
   {
      // it's the configuration updating call
      if (context.Method.EndsWith(nameof(DispatcherGrpcService.ConfigurationCall)))
         return;

      var device = await _workContext.GetCurrentDeviceAsync();
      var serverDeviceConfig = await _genericAttributeService.GetAttributeAsync<long>(device, ClientDefaults.DeviceConfigurationVersion);

      if (!context.GetHttpContext().Request.Headers.TryGetValue(DispatcherDefaults.HeaderConfigurationKey, out var deviceConfiguration)
         || !long.TryParse(deviceConfiguration, out var deviceConfigurationValue)
         || serverDeviceConfig != deviceConfigurationValue)
      {
         var httpContext = context.GetHttpContext();
         httpContext.Response.StatusCode = (int)HttpStatusCode.UpgradeRequired;
         throw new RpcException(new Status(StatusCode.Aborted, "Device configuration must be updated."));
      }
   }

   #endregion;
}
