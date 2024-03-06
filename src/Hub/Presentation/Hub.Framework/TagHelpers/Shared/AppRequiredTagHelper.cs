using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace Hub.Web.Framework.TagHelpers.Shared
{
   /// <summary>
   /// "app-required" tag helper
   /// </summary>
   [HtmlTargetElement("app-required", TagStructure = TagStructure.WithoutEndTag)]
   public class AppRequiredTagHelper : TagHelper
   {
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

         //clear the output
         output.SuppressOutput();

         output.TagName = "span";
         output.TagMode = TagMode.StartTagAndEndTag;
         output.Attributes.SetAttribute("class", "required");
         output.Content.SetContent("*");

         return Task.CompletedTask;
      }

      #endregion
   }
}