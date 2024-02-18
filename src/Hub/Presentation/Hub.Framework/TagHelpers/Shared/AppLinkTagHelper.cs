using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Hub.Web.Framework.UI;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Hub.Web.Framework.TagHelpers.Shared;

/// <summary>
/// CSS bundling tag helper
/// </summary>
[HtmlTargetElement(LINK_TAG_NAME, Attributes = "[rel=stylesheet]")]
public class AppLinkTagHelper : UrlResolutionTagHelper
{
   #region Constants

   private const string LINK_TAG_NAME = "link";
   private const string EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME = "asp-exclude-from-bundle";
   private const string HREF_ATTRIBUTE_NAME = "href";

   #endregion

   #region Fields
   private readonly IAppHtmlHelper _appHtmlHelper;

   #endregion

   #region Ctor

   /// <summary> IoC Ctor </summary>
   public AppLinkTagHelper(HtmlEncoder htmlEncoder,
       IAppHtmlHelper appHtmlHelper,
       IUrlHelperFactory urlHelperFactory) : base(urlHelperFactory, htmlEncoder)
   {
      _appHtmlHelper = appHtmlHelper;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Css (link tag) bundling process  
   /// </summary>
   /// <param name="context"></param>
   /// <param name="output"></param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullException"></exception>
   public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
   {
      if (context is null)
         throw new ArgumentNullException(nameof(context));

      if (output == null)
         throw new ArgumentNullException(nameof(output));

      _appHtmlHelper.AddCssFileParts(Href, string.Empty, ExcludeFromBundle);

      output.SuppressOutput();

      return Task.CompletedTask;
   }

   #endregion

   #region Properties

   /// <summary>
   /// A value indicating if a file should be excluded from the bundle
   /// </summary>
   [HtmlAttributeName(EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME)]
   public bool ExcludeFromBundle { get; set; }

   /// <summary>
   /// Address of the linked resource
   /// </summary>
   [HtmlAttributeName(HREF_ATTRIBUTE_NAME)]
   public string Href { get; set; }

   #endregion
}