using Hub.Core.Caching;

namespace Hub.Web.Infrastructure.Cache;

/// <summary>
/// Application model cache defaults
/// </summary>
public static partial class AppModelCacheDefaults
{
   /// <summary>
   /// Key for home page polls
   /// </summary>
   /// <remarks>
   /// {0} : language ID
   /// </remarks>
   public static CacheKey HomepagePollsModelKey => new("Hub.pres.poll.homepage-{0}", PollsPrefixCacheKey);

   /// <summary>
   /// Key for polls by system name
   /// </summary>
   /// <remarks>
   /// {0} : poll system name
   /// {1} : language ID
   /// </remarks>
   public static CacheKey PollBySystemNameModelKey => new("Hub.pres.poll.systemname-{0}-{1}", PollsPrefixCacheKey);
   public static string PollsPrefixCacheKey => "Hub.pres.poll";

   /// <summary>
   /// Key for blog archive (years, months) block model
   /// </summary>
   /// <remarks>
   /// {0} : language ID
   /// </remarks>
   public static CacheKey BlogMonthsModelKey => new("Hub.pres.blog.months-{0}", BlogPrefixCacheKey);
   public static string BlogPrefixCacheKey => "Hub.pres.blog";

   /// <summary>
   /// Key for home page news
   /// </summary>
   /// <remarks>
   /// {0} : language ID
   /// </remarks>
   public static CacheKey HomepageNewsModelKey => new("Hub.pres.news.homepage-{0}", NewsPrefixCacheKey);
   public static string NewsPrefixCacheKey => "Hub.pres.news";

   /// <summary>
   /// Key for logo
   /// </summary>
   /// <remarks>
   /// {0} : current theme
   /// {1} : is connection SSL secured (included in a picture URL)
   /// </remarks>
   public static CacheKey AppLogoPath => new("Hub.pres.logo-{0}-{1}", AppLogoPathPrefixCacheKey);
   public static string AppLogoPathPrefixCacheKey => "Hub.pres.logo";

   /// <summary>
   /// Key for sitemap on the sitemap page
   /// </summary>
   /// <remarks>
   /// {0} : language id
   /// {1} : roles of the current user
   /// </remarks>
   public static CacheKey SitemapPageModelKey => new("Hub.pres.sitemap.page-{0}-{1}", SitemapPrefixCacheKey);
   /// <summary>
   /// Key for sitemap on the sitemap SEO page
   /// </summary>
   /// <remarks>
   /// {0} : sitemap identifier
   /// {1} : language id
   /// {2} : roles of the current user
   /// </remarks>
   public static CacheKey SitemapSeoModelKey => new("Hub.pres.sitemap.seo-{0}-{1}-{2}", SitemapPrefixCacheKey);
   public static string SitemapPrefixCacheKey => "Hub.pres.sitemap";

   /// <summary>
   /// Key for widget info
   /// </summary>
   /// <remarks>
   /// {0} : current user role IDs hash
   /// {1} : widget zone
   /// {2} : current theme name
   /// </remarks>
   public static CacheKey WidgetModelKey => new("Hub.pres.widget-{0}-{1}-{2}", WidgetPrefixCacheKey);
   public static string WidgetPrefixCacheKey => "Hub.pres.widget";
}
