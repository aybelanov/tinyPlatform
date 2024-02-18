using System.Collections.Generic;

namespace Hub.Services.Users;

/// <summary>
/// User multi-factor authentication info
/// </summary>
public partial class UserMultiFactorAuthenticationInfo
{
   /// <summary>
   /// Default Ctor
   /// </summary>
   public UserMultiFactorAuthenticationInfo()
   {
      CustomValues = new Dictionary<string, object>();
   }

   /// <summary>
   /// Username
   /// </summary>
   public string UserName { get; set; }

   /// <summary>
   /// remember the user
   /// </summary>
   public bool RememberMe { get; set; }

   /// <summary>
   /// return url
   /// </summary>
   public string ReturnUrl { get; set; }

   /// <summary>
   /// You can store any custom value in this property
   /// </summary>
   public Dictionary<string, object> CustomValues { get; set; }
}
