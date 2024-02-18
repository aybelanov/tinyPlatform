namespace Hub.Core.Domain.Users
{
   /// <summary>
   /// User activated event
   /// </summary>
   public class UserActivatedEvent
   {
      /// <summary>
      /// Ctor
      /// </summary>
      /// <param name="user">user</param>
      public UserActivatedEvent(User user)
      {
         User = user;
      }

      /// <summary>
      /// User
      /// </summary>
      public User User
      {
         get;
      }
   }
}
