using System;

namespace Clients.Dash.Services.ErrorServices;

/// <summary>
/// Represents an error event argemnt class
/// </summary>
public class ErrorEventArgs : EventArgs
{
#pragma warning disable CS1591

   public ErrorEventArgs(Exception exception, string message)
   {
      Exception = exception;
      NotificationMessage = message;
   }

#pragma warning restore CS1591

   /// <summary>
   /// Error exception
   /// </summary>
   public Exception Exception { get; set; }

   /// <summary>
   /// Custom error notification message
   /// </summary>
   public string NotificationMessage { get; set; }
}
