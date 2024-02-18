using Shared.Common;

namespace Clients.Dash.Domain;

/// <summary>
/// Represents a user select itme
/// </summary>
public class UserSelectItem : BaseEntity
{
   /// <summary>
   /// Username
   /// </summary>
   public string Username { get; set; }

   /// <summary>
   /// Avatar url
   /// </summary>
   public string AvatarUrl { get; set; }
}
