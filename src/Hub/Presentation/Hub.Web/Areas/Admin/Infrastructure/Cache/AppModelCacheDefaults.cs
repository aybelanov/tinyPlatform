using Hub.Core.Caching;

namespace Hub.Web.Areas.Admin.Infrastructure.Cache;

public static partial class AppModelCacheDefaults
{
   /// <summary>
   /// Key for tinyplat.com news cache
   /// </summary>
   public static CacheKey OfficialNewsModelKey => new("Hub.pres.admin.official.news");
}
