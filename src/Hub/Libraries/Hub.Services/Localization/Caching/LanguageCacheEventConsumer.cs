using Hub.Core.Domain.Localization;
using Hub.Services.Caching;
using System.Threading.Tasks;

namespace Hub.Services.Localization.Caching;

/// <summary>
/// Represents a language cache event consumer
/// </summary>
public partial class LanguageCacheEventConsumer : CacheEventConsumer<Language>
{
   /// <summary>
   /// Clear cache data
   /// </summary>
   /// <param name="entity">Entity</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected override async Task ClearCacheAsync(Language entity)
   {
      await RemoveAsync(AppLocalizationDefaults.LocaleStringResourcesAllPublicCacheKey, entity);
      await RemoveAsync(AppLocalizationDefaults.LocaleStringResourcesAllAdminCacheKey, entity);
      await RemoveAsync(AppLocalizationDefaults.LocaleStringResourcesAllCacheKey, entity);
      await RemoveByPrefixAsync(AppLocalizationDefaults.LocaleStringResourcesByNamePrefix, entity);
   }
}