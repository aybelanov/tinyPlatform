using Shared.Common;

namespace Hub.Services.Users;

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
   public long AvatarPictureId { get; set; }
}
