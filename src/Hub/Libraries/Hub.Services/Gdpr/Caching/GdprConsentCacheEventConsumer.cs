using Hub.Core.Domain.Gdpr;
using Hub.Services.Caching;

namespace Hub.Services.Gdpr.Caching
{
   /// <summary>
   /// Represents a GDPR consent cache event consumer
   /// </summary>
   public partial class GdprConsentCacheEventConsumer : CacheEventConsumer<GdprConsent>
   {
   }
}