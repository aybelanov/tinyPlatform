using Hub.Core.Domain.Seo;
using Hub.Services.Caching;
using System.Threading.Tasks;

namespace Hub.Services.Seo.Caching
{
   /// <summary>
   /// Represents an URL record cache event consumer
   /// </summary>
   public partial class UrlRecordCacheEventConsumer : CacheEventConsumer<UrlRecord>
   {
      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(UrlRecord entity)
      {
         await RemoveAsync(AppSeoDefaults.UrlRecordCacheKey, entity.EntityId, entity.EntityName, entity.LanguageId);
         await RemoveAsync(AppSeoDefaults.UrlRecordBySlugCacheKey, entity.Slug);
      }
   }
}
