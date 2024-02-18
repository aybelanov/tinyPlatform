using Clients.Dash.Services.Localization;
using Clients.Dash.Shared.Communication;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using Radzen;
using Shared.Clients.Configuration;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Clients.Dash.Services.ErrorServices;

/// <summary>
/// Represents a class for handling application error
/// </summary>
public class ErrorService
{
   #region Events

   /// <summary>
   /// Error has occured 
   /// </summary>
   public event ErrorProcessEventHandler ErrorHasOccured;

   #endregion

   #region fields

   private readonly ILogger<ErrorService> _logger;
   private readonly NotificationService _notificationService;
   private readonly DataLoadProcess _dataLoadProcess;
   private readonly Localizer T;
   private readonly NavigationManager _navigationManager;
   private readonly AuthenticationStateProvider _authStateProvider;

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public ErrorService(ILogger<ErrorService> logger,
      NotificationService notificationService,
      DataLoadProcess dataLoadProcess,
      Localizer localizer,
      AuthenticationStateProvider authStateProvider,
      NavigationManager navigationManager)
   {
      _logger = logger;
      _notificationService = notificationService;
      _dataLoadProcess = dataLoadProcess;
      T = localizer;
      _navigationManager = navigationManager;
      _authStateProvider = authStateProvider;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Handles the error
   /// </summary>
   /// <param name="sender">Error sensder</param>
   /// <param name="exception">Error exception</param>
   /// <param name="notificationMessage">Custom error notification message</param>
   public async Task HandleError(object sender, Exception exception, string notificationMessage = null)
   {
      ErrorHasOccured?.Invoke(sender, new ErrorEventArgs(exception, notificationMessage));

      _dataLoadProcess.Off();
      _dataLoadProcess.ClearLoading();

      if (exception is RpcException rpcException)
      {
         // demo user
         if (rpcException.StatusCode == StatusCode.PermissionDenied && rpcException.Status.Detail.Equals(HttpStatusCode.MethodNotAllowed.ToString()))
         {
            var state = await _authStateProvider.GetAuthenticationStateAsync();
            if (state != null && state.User?.IsInRole(UserDefaults.DemoRoleName) == true)
            {
               _notificationService.Notify(NotificationSeverity.Info, T["Exception.Rpc.Demo"], duration: 5000);
               return;
            }
         }

         _notificationService.Notify(NotificationSeverity.Error, T[$"Exception.Rpc.{rpcException.StatusCode}", rpcException.Status.Detail], duration: -1d);

         if (rpcException.StatusCode == StatusCode.Unauthenticated || rpcException.InnerException is AccessTokenNotAvailableException)
            _navigationManager.NavigateToLogout("authentication/logout");
      }
      else
      {
         var message = (notificationMessage ?? string.Empty) + " " + (exception.Message ?? string.Empty);
         _notificationService.Notify(NotificationSeverity.Error, T["Exception.Application.Message", message], duration: -1d);
      }

      _logger.LogWarning(exception, message: exception.Message + "\r\n " + (exception?.InnerException?.Message ?? string.Empty));

      //await Task.CompletedTask;
   }

   #endregion
}