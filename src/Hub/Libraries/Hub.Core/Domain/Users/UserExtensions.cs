using System;

namespace Hub.Core.Domain.Users;

/// <summary>
/// User extensions
/// </summary>
public static class UserExtensions
{
   /// <summary>
   /// Gets a value indicating whether user a search engine
   /// </summary>
   /// <param name="user">User</param>
   /// <returns>Result</returns>
   public static bool IsSearchEngineAccount(this User user)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      if (!user.IsSystemAccount || string.IsNullOrEmpty(user.SystemName))
         return false;

      var result = user.SystemName.Equals(AppUserDefaults.SearchEngineUserName, StringComparison.InvariantCultureIgnoreCase);

      return result;
   }

   /// <summary>
   /// Gets a value indicating whether the user is a built-in record for background tasks
   /// </summary>
   /// <param name="user">User</param>
   /// <returns>Result</returns>
   public static bool IsBackgroundTaskAccount(this User user)
   {
      if (user == null)
         throw new ArgumentNullException(nameof(user));

      if (!user.IsSystemAccount || string.IsNullOrEmpty(user.SystemName))
         return false;

      var result = user.SystemName.Equals(AppUserDefaults.BackgroundTaskUserName, StringComparison.InvariantCultureIgnoreCase);

      return result;
   }
}