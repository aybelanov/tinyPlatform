using System;
using System.Net.Http;
using System.Threading.Tasks;
using Hub.Core;

namespace Hub.Services.Common;

/// <summary>
/// Represents the HTTP client to request current platform
/// </summary>
public partial class PlatformHttpClient
{
   #region Fields

   private readonly HttpClient _httpClient;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   /// <param name="httpClient"></param>
   /// <param name="webHelper"></param>
   public PlatformHttpClient(HttpClient httpClient, IWebHelper webHelper)
   {
      //configure client
      httpClient.BaseAddress = new Uri(webHelper.GetAppLocation());
      
      _httpClient = httpClient;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Keep the current platform site alive
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the asynchronous task whose result determines that request completed
   /// </returns>
   public virtual async Task KeepAliveAsync()
   {
      await _httpClient.GetStringAsync(HubCommonDefaults.KeepAlivePath);
   }

   #endregion
}