using Radzen;
using Shared.Clients;
using System.Threading.Tasks;

namespace Clients.Dash.Services.Helpers;

/// <summary>
/// Repreents a common helper service interface
/// </summary>
public interface IHelperService
{
   /// <summary>
   /// Gets localized online status string
   /// </summary>
   /// <param name="onlineStatus">Online status</param>
   /// <returns>Locale string</returns>
   string GetOnlineStatusLocale(OnlineStatus onlineStatus);


   /// <summary>
   /// Gets a badge style
   /// </summary>
   /// <param name="status">Online status</param>
   /// <returns></returns>
   (BadgeStyle, string) GetBadgeStatus(OnlineStatus status);

   /// <summary>
   /// Gets localized online status string
   /// </summary>
   /// <param name="onlineStatus">Online status</param>
   /// <returns>Locale string</returns>
   Task<string> GetOnlineStatusLocaleAsync(OnlineStatus onlineStatus);
}
