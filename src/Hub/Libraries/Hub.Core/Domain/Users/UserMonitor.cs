using Shared.Common;

namespace Hub.Core.Domain.Users;

/// <summary>
/// Represents a user to monitor map 
/// </summary>
public class UserMonitor : BaseEntity
{
   /// <summary>
   /// Get or set a user identifier
   /// </summary>
   public long UserId { get; set; }

   /// <summary>
   /// Get or set a monitor indentifier
   /// </summary>
   public long MonitorId { get; set; }

   /// <summary>
   /// Show shared monitor in the main menu
   /// </summary>
   public bool ShowInMenu { get; set; }  

   /// <summary>
   /// Display order (in main menu)
   /// </summary>
   public int DisplayOrder { get; set; }
}
