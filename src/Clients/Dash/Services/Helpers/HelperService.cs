using Clients.Dash.Services.Localization;
using Microsoft.JSInterop;
using Radzen;
using Shared.Clients;
using System.Threading.Tasks;

namespace Clients.Dash.Services.Helpers;

/// <summary>
/// Repreents a common helper service 
/// </summary>
public class HelperService : IHelperService
{
   #region fields

   private readonly Localizer _localizer;
   private readonly IJSRuntime _js;

   #endregion

   #region Ctors

   /// <summary>
   /// IoC
   /// </summary>
   public HelperService(Localizer localizer, IJSRuntime js)
   {
      _js = js;
      _localizer = localizer;
   }

   #endregion

   #region Methods


   /// <summary>
   /// Gets localized online status string
   /// </summary>
   /// <param name="onlineStatus">Online status</param>
   /// <returns>Locale string</returns>
   public string GetOnlineStatusLocale(OnlineStatus onlineStatus)
   {
      var onlineStatusString = onlineStatus switch
      {
         OnlineStatus.Offline => _localizer[$"OnlineStatus.{nameof(OnlineStatus.Offline)}"],
         OnlineStatus.Online => _localizer[$"OnlineStatus.{nameof(OnlineStatus.Online)}"],
         OnlineStatus.BeenRecently => _localizer[$"OnlineStatus.{nameof(OnlineStatus.BeenRecently)}"],
         OnlineStatus.NoActivities => _localizer[$"OnlineStatus.{nameof(OnlineStatus.NoActivities)}"],
         _ => _localizer[$"OnlineStatus.{nameof(OnlineStatus.NoActivities)}"]
      };

      return onlineStatusString.ToString();
   }

   /// <summary>
   /// Gets a badge style
   /// </summary>
   /// <param name="status">Online status</param>
   /// <returns></returns>
   public (BadgeStyle, string) GetBadgeStatus(OnlineStatus status)
   {
      var style = status switch
      {
         OnlineStatus.Offline => (BadgeStyle.Danger, GetOnlineStatusLocale(OnlineStatus.Offline)),
         OnlineStatus.Online => (BadgeStyle.Success, GetOnlineStatusLocale(OnlineStatus.Online)),
         OnlineStatus.BeenRecently => (BadgeStyle.Warning, GetOnlineStatusLocale(OnlineStatus.BeenRecently)),
         OnlineStatus.NoActivities => (BadgeStyle.Secondary, GetOnlineStatusLocale(OnlineStatus.NoActivities)),
         _ => (BadgeStyle.Secondary, GetOnlineStatusLocale(OnlineStatus.NoActivities))
      };

      return style;
   }

   /// <summary>
   /// Gets localized online status string
   /// </summary>
   /// <param name="onlineStatus">Online status</param>
   /// <returns>Locale string</returns>
   public Task<string> GetOnlineStatusLocaleAsync(OnlineStatus onlineStatus)
   {
      return Task.FromResult(GetOnlineStatusLocale(onlineStatus));
   }
   #endregion
}
