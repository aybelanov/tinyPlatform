using Clients.Dash.Services.Helpers;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Clients.Dash.Infrastructure;

/// <summary>
/// Application extensions
/// </summary>
public static class AppExtensions
{
   /// <summary>
   /// Handles
   /// </summary>
   /// <param name="response"></param>
   public static void HandleNotSuccesResponse(this HttpResponseMessage response)
   {
      var status = response.StatusCode switch
      {
         HttpStatusCode.Unauthorized => new(StatusCode.Unauthenticated, HttpStatusCode.Unauthorized.ToString()),
         HttpStatusCode.Forbidden => new(StatusCode.PermissionDenied, HttpStatusCode.Forbidden.ToString()),
         HttpStatusCode.MethodNotAllowed => new(StatusCode.PermissionDenied, HttpStatusCode.MethodNotAllowed.ToString()),
         HttpStatusCode.NotAcceptable => new(StatusCode.Internal, HttpStatusCode.NotAcceptable.ToString()),
         HttpStatusCode.NotFound => new(StatusCode.NotFound, HttpStatusCode.NotFound.ToString()),
         // TODO check for grpc (webapi calls an exception)
         //HttpStatusCode.NoContent => HttpStatusCode.NoContent.ToString(),
         _ => new Status()
      };

      if (status.Detail is not null)
         throw new RpcException(status);
   }


   /// <summary>
   /// Handles 500 Internal Server Error
   /// </summary>
   /// <param name="response">Server response</param>
   /// <returns></returns>
   public static async Task HandleInternalServerError(this HttpResponseMessage response)
   {
      if (response.StatusCode == HttpStatusCode.InternalServerError)
      {
         var message = await response.Content.ReadAsStringAsync();

         // TODO add other db constraint message
         if (message.Contains("UNIQUE constraint failed"))
            throw new RpcException(new Status(StatusCode.AlreadyExists, "Error.HttpResponse.500EntityAlreadyExists"));
         if (message.StartsWith("System.NotImplementedException"))
            throw new RpcException(new Status(StatusCode.Unimplemented, "Error.HttpResponse.500NotImplemented"));
         else
            // yet unhandled and unparsed exception
            throw new Exception(message);
      }
   }

   /// <summary>
   /// Gets element boundings
   /// </summary>
   /// <param name="element">Element</param>
   /// <returns>Element boundings</returns>
   /// <remarks>
   /// <see href="https://developer.mozilla.org/en-US/docs/Web/API/Element/getBoundingClientRect"/>
   /// </remarks>
   public static ElementBoundings GetElementBounds(this ElementReference element)
   {
      using var scope = Singleton<IServiceProvider>.Instance.GetRequiredService<IServiceScopeFactory>().CreateScope();
      var js = (IJSInProcessRuntime)scope.ServiceProvider.GetRequiredService<IJSRuntime>();
      var bounds = js.Invoke<ElementBoundings>("appjs.getElementSize", element);
      return bounds;
   }
}
