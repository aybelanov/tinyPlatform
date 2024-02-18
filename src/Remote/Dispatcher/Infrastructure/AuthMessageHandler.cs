using Devices.Dispatcher.Services.Authentication;
using Devices.Dispatcher.Services.Settings;
using Shared.Devices;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Devices.Dispatcher.Infrastructure;

/// <summary>
/// Authetication and configuration version handler for http clients
/// </summary>
public class AuthMessageHandler : DelegatingHandler
{
   #region fields

   private readonly ITokenProvider _tokenProvider;
   private readonly ISettingService _settingService;

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   /// <param name="tokenProvider"></param>
   /// <param name="settingService"></param>
   public AuthMessageHandler(ITokenProvider tokenProvider, ISettingService settingService)
   {
      _tokenProvider = tokenProvider;
      _settingService = settingService;
   }

   #endregion

   #region Methods

   /// <inheritdoc/>
   protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
   {
      var token = await _tokenProvider.GetTokenAsync();
      request.Headers.Add("Authorization", $"Bearer {token}");

      var configurationVersion = await _settingService.GetConfigurationVersion();
      request.Headers.Add(DispatcherDefaults.HeaderConfigurationKey, configurationVersion.ToString());

      var response = await base.SendAsync(request, cancellationToken);
      if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
         await _settingService.DeleteSettingAsync<TokenStore>();

      return response;
   }

   #endregion
}