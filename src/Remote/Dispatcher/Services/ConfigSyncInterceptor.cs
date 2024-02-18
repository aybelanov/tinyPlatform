using Grpc.Core.Interceptors;
using Grpc.Core;
using static Grpc.Core.Interceptors.Interceptor;
using System.Threading.Tasks;
using System;

namespace Devices.Dispatcher.Services;

#pragma warning disable CS1591

public class ConfigSyncInterceptor : Interceptor
{
   public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
   TRequest request,
   ClientInterceptorContext<TRequest, TResponse> context,
   AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
   {
      var call = continuation(request, context);

      return new AsyncUnaryCall<TResponse>(
          HandleResponse(call.ResponseAsync),
          call.ResponseHeadersAsync,
          HandleStatus(call.GetStatus),
          call.GetTrailers,
          call.Dispose);
   }

   private async Task<TResponse> HandleResponse<TResponse>(Task<TResponse> inner)
   {
      try
      {
         return await inner;
      }
      catch (Exception ex)
      {
         throw new InvalidOperationException("Custom error", ex);
      }
   }

   private Func<Status> HandleStatus(Func<Status> status)
   {
      try
      {
         return status;
      }
      catch (Exception ex)
      {
         throw new InvalidOperationException("Custom error", ex);
      }
   }
}
