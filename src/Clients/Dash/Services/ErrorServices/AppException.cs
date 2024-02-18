using System;
using System.Net;

namespace Clients.Dash.Services.ErrorServices;

/// <summary>
/// Represents aplication exception 
/// </summary>
public class AppException : Exception
{
   /// <summary>
   /// Http response status code
   /// </summary>
   public HttpStatusCode StatusCode { get; set; }

   #region Ctors
#pragma warning disable CS1591

   public AppException() : base()
   {

   }

   public AppException(string message) : base(message)
   {

   }

   public AppException(string message, Exception innerException) : base(message, innerException)
   {

   }

#pragma warning restore CS1591
   #endregion
}
