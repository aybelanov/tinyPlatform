﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Hub.Web.Framework.TagHelpers.Public
{
   /// <summary>
   /// "textarea" tag helper
   /// </summary>
   [HtmlTargetElement("textarea", Attributes = FOR_ATTRIBUTE_NAME)]
   public class TextAreaTagHelper : Microsoft.AspNetCore.Mvc.TagHelpers.TextAreaTagHelper
   {
      #region Constants

      private const string FOR_ATTRIBUTE_NAME = "asp-for";
      private const string DISABLED_ATTRIBUTE_NAME = "asp-disabled";

      #endregion

      #region Properties

      /// <summary>
      /// Indicates whether the input is disabled
      /// </summary>
      [HtmlAttributeName(DISABLED_ATTRIBUTE_NAME)]
      public string IsDisabled { set; get; }

      #endregion

      #region Ctor

      /// <summary> IoC Ctor </summary>
      public TextAreaTagHelper(IHtmlGenerator generator) : base(generator)
      {
      }

      #endregion

      #region Methods

      /// <summary>
      /// Asynchronously executes the tag helper with the given context and output
      /// </summary>
      /// <param name="context">Contains information associated with the current HTML tag</param>
      /// <param name="output">A stateful HTML element used to generate an HTML tag</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
      {
         if (context == null)
            throw new ArgumentNullException(nameof(context));

         if (output == null)
            throw new ArgumentNullException(nameof(output));

         //add disabled attribute
         if (bool.TryParse(IsDisabled, out var disabled) && disabled)
            output.Attributes.Add(new TagHelperAttribute("disabled", "disabled"));

         await base.ProcessAsync(context, output);
      }

      #endregion
   }
}