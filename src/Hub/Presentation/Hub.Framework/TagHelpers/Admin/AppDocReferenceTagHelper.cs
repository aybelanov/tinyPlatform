﻿using Hub.Core.Domain.Common;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace Hub.Web.Framework.TagHelpers.Admin
{
   /// <summary>
   /// "app-doc-reference" tag helper
   /// </summary>
   [HtmlTargetElement("app-doc-reference", Attributes = STRING_RESOURCE_ATTRIBUTE_NAME, TagStructure = TagStructure.WithoutEndTag)]
   public class AppDocReferenceTagHelper : TagHelper
   {
      #region Constants

      private const string STRING_RESOURCE_ATTRIBUTE_NAME = "asp-string-resource";
      private const string ADD_WRAPPER_ATTRIBUTE_NAME = "asp-add-wrapper";

      #endregion

      #region Properties

      /// <summary>
      /// String resource value
      /// </summary>
      [HtmlAttributeName(STRING_RESOURCE_ATTRIBUTE_NAME)]
      public string StringResource { get; set; }

      /// <summary>
      /// Indicates whether the wrapper tag should be added
      /// </summary>
      [HtmlAttributeName(ADD_WRAPPER_ATTRIBUTE_NAME)]
      public bool AddWrapper { get; set; } = true;

      #endregion

      #region Fields

      private readonly AdminAreaSettings _adminAreaSettings;

      #endregion

      #region Ctor

      /// <summary> IoC Ctor </summary>
      public AppDocReferenceTagHelper(AdminAreaSettings adminAreaSettings)
      {
         _adminAreaSettings = adminAreaSettings;
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

         //clear the output
         output.SuppressOutput();

         if (_adminAreaSettings.ShowDocumentationReferenceLinks)
         {
            //add wrapper
            if (AddWrapper)
            {
               output.TagName = "div";
               output.TagMode = TagMode.StartTagAndEndTag;
               output.Attributes.SetAttribute("class", "documentation-reference");
            }

            var hintHtml = $"<span>{StringResource}</span>";
            output.Content.AppendHtml(hintHtml);
         }

         return Task.CompletedTask;
      }

      #endregion
   }
}