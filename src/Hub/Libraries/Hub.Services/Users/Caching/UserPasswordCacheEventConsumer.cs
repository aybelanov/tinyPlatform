using Hub.Core.Domain.Users;
using Hub.Services.Caching;

namespace Hub.Services.Users.Caching
{
   /// <summary>
   /// Represents a user password cache event consumer
   /// </summary>
   public partial class UserPasswordCacheEventConsumer : CacheEventConsumer<UserPassword>
   {
   }
}