using Hub.Core.Domain.Common;
using Hub.Services.Caching;

namespace Hub.Services.Common.Caching
{
   /// <summary>
   /// Represents a search term cache event consumer
   /// </summary>
   public partial class SearchTermCacheEventConsumer : CacheEventConsumer<SearchTerm>
   {
   }
}
