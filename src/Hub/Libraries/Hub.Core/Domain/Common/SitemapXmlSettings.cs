using System.Collections.Generic;
using Hub.Core.Configuration;

namespace Hub.Core.Domain.Common;

/// <summary>
/// Represent sitemap.xml settings
/// </summary>
public class SitemapXmlSettings : ISettings
{
   /// <summary>
   /// Deafult Ctor
   /// </summary>
   public SitemapXmlSettings()
   {
      SitemapCustomUrls = new List<string>();
   }

   /// <summary>
   /// Gets or sets a value indicating whether sitemap.xml is enabled
   /// </summary>
   public bool SitemapXmlEnabled { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to include blog posts to sitemap.xml
   /// </summary>
   public bool SitemapXmlIncludeBlogPosts { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to include categories to sitemap.xml
   /// </summary>
   public bool SitemapXmlIncludeCategories { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to include custom urls to sitemap.xml
   /// </summary>
   public bool SitemapXmlIncludeCustomUrls { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to include news to sitemap.xml
   /// </summary>
   public bool SitemapXmlIncludeNews { get; set; }
   /// <summary>
   /// Gets or sets a value indicating whether to include topics to sitemap.xml
   /// </summary>
   public bool SitemapXmlIncludeTopics { get; set; }

   /// <summary>
   /// A list of custom URLs to be added to sitemap.xml (include page names only)
   /// </summary>
   public List<string> SitemapCustomUrls { get; set; }
}