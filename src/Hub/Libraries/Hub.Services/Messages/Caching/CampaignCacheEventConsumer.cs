using Hub.Core.Domain.Messages;
using Hub.Services.Caching;

namespace Hub.Services.Messages.Caching
{
   /// <summary>
   /// Represents a campaign cache event consumer
   /// </summary>
   public partial class CampaignCacheEventConsumer : CacheEventConsumer<Campaign>
   {
   }
}
