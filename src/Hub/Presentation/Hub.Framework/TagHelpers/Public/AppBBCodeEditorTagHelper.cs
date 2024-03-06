using Hub.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace Hub.Web.Framework.TagHelpers.Public
{
   /// <summary>
   /// "app-bb-code-editor" tag helper
   /// </summary>
   [HtmlTargetElement("app-bb-code-editor", Attributes = FOR_ATTRIBUTE_NAME, TagStructure = TagStructure.WithoutEndTag)]
   public class AppBBCodeEditorTagHelper : TagHelper
   {
      #region Constants

      private const string FOR_ATTRIBUTE_NAME = "asp-for";

      #endregion

      #region Properties

      /// <summary>
      /// An expression to be evaluated against the current model
      /// </summary>
      [HtmlAttributeName(FOR_ATTRIBUTE_NAME)]
      public ModelExpression For { get; set; }

      /// <summary>
      /// ViewContext
      /// </summary>
      [HtmlAttributeNotBound]
      [ViewContext]
      public ViewContext ViewContext { get; set; }

      #endregion

      #region Fields

      private readonly IWebHelper _webHelper;

      #endregion

      #region Ctor

      /// <summary> IoC Ctor </summary>
      public AppBBCodeEditorTagHelper(IWebHelper webHelper)
      {
         _webHelper = webHelper;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Asynchronously executes the tag helper with the given context and output
      /// </summary>
      /// <param name="context">Contains information associated with the current HTML tag</param>
      /// <param name="output">A stateful HTML element used to generate an HTML tag</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
      {
         if (context == null)
            throw new ArgumentNullException(nameof(context));

         if (output == null)
            throw new ArgumentNullException(nameof(output));

         output.TagName = "div";
         output.TagMode = TagMode.StartTagAndEndTag;
         output.Attributes.Add("class", "bb-code-editor-wrapper");

         var platformLocation = _webHelper.GetAppLocation();

         var bbEditorWebRoot = $"{platformLocation}js/";

         var script1 = new TagBuilder("script");
         script1.Attributes.Add("src", $"{platformLocation}js/bbeditor/ed.js");

         var script2 = new TagBuilder("script");
         script2.Attributes.Add("language", "javascript");
         script2.InnerHtml.AppendHtml($"edToolbar('{For.Name}','{bbEditorWebRoot}');");

         output.Content.AppendHtml(script1);
         output.Content.AppendHtml(script2);

         return Task.CompletedTask;
      }

      #endregion
   }
}