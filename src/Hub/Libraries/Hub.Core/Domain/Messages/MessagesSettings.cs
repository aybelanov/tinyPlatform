using Hub.Core.Configuration;

namespace Hub.Core.Domain.Messages;

/// <summary>
/// Messages settings
/// </summary>
public class MessagesSettings : ISettings
{
   /// <summary>
   /// A value indicating whether popup notifications set as default 
   /// </summary>
   public bool UsePopupNotifications { get; set; }
}
