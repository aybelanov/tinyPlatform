using Hub.Core.Domain.Localization;
using Hub.Services.Caching;
using Hub.Services.Localization;
using System.Threading.Tasks;

namespace Hub.Services.Localization.Caching;

/// <summary>
/// Represents a locale string resource cache event consumer
/// </summary>
public partial class LocaleStringResourceCacheEventConsumer : CacheEventConsumer<LocaleStringResource>
{
   /// <summary>
   /// Clear cache by entity event type
   /// </summary>
   /// <param name="entity">Entity</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   protected override async Task ClearCacheAsync(LocaleStringResource entity)
   {
      await RemoveAsync(AppLocalizationDefaults.LocaleStringResourcesAllPublicCacheKey, entity.LanguageId);
      await RemoveAsync(AppLocalizationDefaults.LocaleStringResourcesAllAdminCacheKey, entity.LanguageId);
      await RemoveAsync(AppLocalizationDefaults.LocaleStringResourcesAllCacheKey, entity.LanguageId);
      await RemoveByPrefixAsync(AppLocalizationDefaults.LocaleStringResourcesByNamePrefix, entity.LanguageId);
   }
}
