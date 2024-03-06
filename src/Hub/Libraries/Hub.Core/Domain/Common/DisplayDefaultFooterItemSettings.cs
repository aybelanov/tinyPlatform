using Hub.Core.Configuration;

namespace Hub.Core.Domain.Common;

/// <summary>
/// Display default menu item settings
/// </summary>
public class DisplayDefaultFooterItemSettings : ISettings
{
   /// <summary>
   /// Gets or sets a value indicating whether to display "sitemap" footer item
   /// </summary>
   public bool DisplaySitemapFooterItem { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to display "contact us" footer item
   /// </summary>
   public bool DisplayContactUsFooterItem { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to display "search search" footer item
   /// </summary>
   public bool DisplaySearchFooterItem { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to display "news" footer item
   /// </summary>
   public bool DisplayNewsFooterItem { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to display "blog" footer item
   /// </summary>
   public bool DisplayBlogFooterItem { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to display "forums" footer item
   /// </summary>
   public bool DisplayForumsFooterItem { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to display "documentations" footer item
   /// </summary>
   public bool DisplayDocumentationFooterItem { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to display "user info" footer item
   /// </summary>
   public bool DisplayUserInfoFooterItem { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to display "user addresses" footer item
   /// </summary>
   public bool DisplayUserAddressesFooterItem { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to display "wishlist" footer item
   /// </summary>
   public bool DisplayWishlistFooterItem { get; set; }
}
