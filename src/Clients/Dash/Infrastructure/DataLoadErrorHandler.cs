using Clients.Dash.Shared.Communication;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Clients.Dash.Infrastructure;

/// <summary>
/// Represents a class that handles data load erros
/// </summary>
public class DataLoadErrorHandler : DelegatingHandler, IDisposable
{
   #region fields

   private readonly DataLoadProcess _progressBar;
   private readonly ILogger<DataLoadErrorHandler> _logger;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public DataLoadErrorHandler(DataLoadProcess progressBar, ILogger<DataLoadErrorHandler> logger)
   {
      _progressBar = progressBar;
      _logger = logger;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Data load error handler
   /// </summary>
   /// <param name="request"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
   {
      try
      {
         _progressBar.On();

         var response = await base.SendAsync(request, cancellationToken);
         await response.HandleInternalServerError();
         response.HandleNotSuccesResponse();
         response.EnsureSuccessStatusCode();

         _progressBar.Off();

         return response;
      }
      catch (HttpRequestException httpEx) when (httpEx.Message.Contains("TypeError: Failed to fetch"))
      {
         throw new RpcException(new Status(StatusCode.Unavailable, "Error data fetch."));
      }
      catch (Exception abortEx) when (abortEx.Message.Contains("The operation was canceled"))
      {
         throw new RpcException(new Status(StatusCode.Cancelled, "Operation was canceled"));
      }
      catch
      {
         throw;
      }
      finally
      {
         _progressBar.ClearLoading();
      }
   }

   #endregion
}