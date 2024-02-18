namespace Hub.Core.Domain.Seo
{
   /// <summary>
   /// Represents a page title SEO adjustment
   /// </summary>
   public enum PageTitleSeoAdjustment
   {
      /// <summary>
      /// Pagename comes after platformname
      /// </summary>
      PagenameAfterPlatformname = 0,

      /// <summary>
      /// Platformname comes after pagename
      /// </summary>
      PlatformnameAfterPagename = 10
   }
}
