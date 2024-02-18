using Hub.Core.Domain.Forums;
using Hub.Services.Caching;

namespace Hub.Services.Forums.Caching
{
   /// <summary>
   /// Represents a private message cache event consumer
   /// </summary>
   public partial class PrivateMessageCacheEventConsumer : CacheEventConsumer<PrivateMessage>
   {
   }
}
