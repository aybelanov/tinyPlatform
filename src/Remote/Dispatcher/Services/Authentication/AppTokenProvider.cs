using Devices.Dispatcher.Configuration;
using Devices.Dispatcher.Services.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Devices.Dispatcher.Services.Authentication;

/// <summary>
/// Representa token interface implementation 
/// </summary>
public class AppTokenProvider : ITokenProvider
{
   #region Fields

   private readonly IServiceScopeFactory _scopeFactory;
   private readonly HubConnections _hubConnections;
   private readonly IHttpClientFactory _clientFactory;
   private readonly ILogger<AppTokenProvider> _logger;   

   private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public AppTokenProvider(IServiceScopeFactory scopeFactory, HubConnections hubConnections, ILogger<AppTokenProvider> logger, IHttpClientFactory clientFactory)
   {
      _clientFactory = clientFactory;
      _scopeFactory = scopeFactory;
      _hubConnections = hubConnections;
      _logger = logger;
   }

   #endregion

   #region Methods

   /// <inheritdoc/>
   public async Task<string> GetTokenAsync()
   {
      await _semaphore.WaitAsync();
      var now = DateTimeOffset.UtcNow;
      try
      {
         using var scope = _scopeFactory.CreateScope();
         var settingService = scope.ServiceProvider.GetService<ISettingService>();
         var accessToken = await settingService.LoadSettingAsync<TokenStore>();

         if (string.IsNullOrWhiteSpace(accessToken?.AccessToken) || now >= accessToken.CreatedOn.ToDateTimeFromUinxEpoch().AddSeconds(accessToken.ExpiresIn).AddMinutes(-5))
         {
            accessToken = await RequestTokenAsync();
            await settingService.SaveSettingAsync(accessToken);
         }

         return accessToken.AccessToken;
      }
      finally { _semaphore.Release(); }
   }

   #endregion

   #region Utils

   private async Task<TokenStore> RequestTokenAsync()
   {
      var requestMessage = PrepareRequestMessage();
      var httpClient = _clientFactory.CreateClient(Defaults.TokenHttpClient);
      using var request = new HttpRequestMessage(HttpMethod.Post, string.Empty) { Content = new FormUrlEncodedContent(requestMessage) };
      using var response = await httpClient.SendAsync(request);
      response.EnsureSuccessStatusCode();
      using var stream = await response.Content.ReadAsStreamAsync();
      var token = await JsonSerializer.DeserializeAsync<TokenStore>(stream);
      
      if (string.IsNullOrWhiteSpace(token.Error))
      {
         return token;
      }

      throw new Exception(token.Error);
   }


   private Dictionary<string, string> PrepareRequestMessage()
   {
      var requestData = new Dictionary<string, string>()
      {
         ["client_id"] = _hubConnections.ClientId,
         ["device_id"] = _hubConnections.SystemName,
         ["client_secret"] = _hubConnections.SecretCredential,
         ["grant_type"] = "client_credentials"
      };

      return requestData;
   }

   #endregion
}
