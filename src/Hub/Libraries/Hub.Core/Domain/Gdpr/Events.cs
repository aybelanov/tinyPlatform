namespace Hub.Core.Domain.Gdpr
{
   /// <summary>
   /// User permanently deleted (GDPR)
   /// </summary>
   public class UserPermanentlyDeleted
   {
      /// <summary>
      /// Ctor
      /// </summary>
      /// <param name="userId">User identifier</param>
      /// <param name="email">Email</param>
      public UserPermanentlyDeleted(long userId, string email)
      {
         UserId = userId;
         Email = email;
      }

      /// <summary>
      /// User identifier
      /// </summary>
      public long UserId { get; }

      /// <summary>
      /// Email
      /// </summary>
      public string Email { get; }
   }
}