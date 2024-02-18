using Hub.Core.Domain.Messages;
using Hub.Services.Caching;

namespace Hub.Services.Messages.Caching
{
   /// <summary>
   /// Represents news letter subscription cache event consumer
   /// </summary>
   public partial class NewsLetterSubscriptionCacheEventConsumer : CacheEventConsumer<NewsLetterSubscription>
   {
   }
}
