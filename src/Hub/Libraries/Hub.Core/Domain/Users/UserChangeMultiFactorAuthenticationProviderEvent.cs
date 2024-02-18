namespace Hub.Core.Domain.Users
{
   /// <summary>
   /// "User is change multi-factor authentication provider" event
   /// </summary>
   public class UserChangeMultiFactorAuthenticationProviderEvent
   {
      /// <summary>
      /// Ctor
      /// </summary>
      /// <param name="user">User</param>
      public UserChangeMultiFactorAuthenticationProviderEvent(User user)
      {
         User = user;
      }

      /// <summary>
      /// Get or set the user
      /// </summary>
      public User User { get; }
   }
}
