using System.Collections.Generic;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Common;

public partial record FooterModel : BaseAppModel
{
   public FooterModel()
   {
      Topics = new List<FooterTopicModel>();
   }

   public string ServiceName { get; set; }
   public bool IsHomePage { get; set; }
   public bool WishlistEnabled { get; set; }
   public bool SitemapEnabled { get; set; }
   public bool SearchEnabled { get; set; }
   public bool NewsEnabled { get; set; }
   public bool BlogEnabled { get; set; }
   public bool ForumEnabled { get; set; }
   public bool HidePoweredBy { get; set; }

   public long WorkingLanguageId { get; set; }

   public IList<FooterTopicModel> Topics { get; set; }

   public bool DisplaySitemapFooterItem { get; set; }
   public bool DisplayContactUsFooterItem { get; set; }
   public bool DisplayNewsFooterItem { get; set; }
   public bool DisplayBlogFooterItem { get; set; }
   public bool DisplayForumsFooterItem { get; set; }
   public bool DisplayDocumentationFooterItem { get; set; }
   public bool DisplayUserInfoFooterItem { get; set; }
   public bool DisplayUserAddressesFooterItem { get; set; }
   public bool DisplayWishlistFooterItem { get; set; }

   public bool DisplaySearchFooterItem { get;set; }

   #region Nested classes

   public record FooterTopicModel : BaseAppEntityModel
   {
      public string Name { get; set; }
      public string SeName { get; set; }

      public bool IncludeInFooterColumn1 { get; set; }
      public bool IncludeInFooterColumn2 { get; set; }
      public bool IncludeInFooterColumn3 { get; set; }
   }

   #endregion
}