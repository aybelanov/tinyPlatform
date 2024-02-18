using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Hub.Web.Framework.TagHelpers.Admin
{
   /// <summary>
   /// "app-cards" tag helper
   /// </summary>
   [HtmlTargetElement("app-cards", Attributes = ID_ATTRIBUTE_NAME)]
   public class AppCardsTagHelper : TagHelper
   {
      #region Constants

      private const string ID_ATTRIBUTE_NAME = "id";

      #endregion

      #region Properties

      /// <summary>
      /// ViewContext
      /// </summary>
      [HtmlAttributeNotBound]
      [ViewContext]
      public ViewContext ViewContext { get; set; }

      #endregion
   }
}