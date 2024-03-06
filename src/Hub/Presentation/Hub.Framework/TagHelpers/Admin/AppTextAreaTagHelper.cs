﻿using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace Hub.Web.Framework.TagHelpers.Admin
{
   /// <summary>
   /// "app-textarea" tag helper
   /// </summary>
   [HtmlTargetElement("app-textarea", Attributes = FOR_ATTRIBUTE_NAME)]
   public class AppTextAreaTagHelper : TextAreaTagHelper
   {
      #region Constants

      private const string FOR_ATTRIBUTE_NAME = "asp-for";
      private const string REQUIRED_ATTRIBUTE_NAME = "asp-required";
      private const string CUSTOM_HTML_ATTRIBUTES = "html-attributes";

      #endregion

      #region Properties

      /// <summary>
      /// Custom html attributes
      /// </summary>
      [HtmlAttributeName(CUSTOM_HTML_ATTRIBUTES)]
      public object CustomHtmlAttributes { set; get; }

      /// <summary>
      /// Indicates whether the field is required
      /// </summary>
      [HtmlAttributeName(REQUIRED_ATTRIBUTE_NAME)]
      public string IsRequired { set; get; }

      #endregion

      #region Ctor

      /// <summary> IoC Ctor </summary>
      public AppTextAreaTagHelper(IHtmlGenerator generator) : base(generator)
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

         //tag details
         output.TagName = "textarea";
         output.TagMode = TagMode.StartTagAndEndTag;

         //merge classes
         var classValue = output.Attributes.ContainsName("class")
             ? $"{output.Attributes["class"].Value} form-control"
             : "form-control";
         output.Attributes.SetAttribute("class", classValue);

         //set custom html attributes
         var htmlAttributesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(CustomHtmlAttributes);
         if (htmlAttributesDictionary?.Count > 0)
            foreach (var (key, value) in htmlAttributesDictionary)
               output.Attributes.Add(key, value);

         //additional parameters
         var rowsNumber = output.Attributes.ContainsName("rows") ? output.Attributes["rows"].Value : 4;
         output.Attributes.SetAttribute("rows", rowsNumber);
         var colsNumber = output.Attributes.ContainsName("cols") ? output.Attributes["cols"].Value : 20;
         output.Attributes.SetAttribute("cols", colsNumber);

         //required asterisk
         if (bool.TryParse(IsRequired, out var required) && required)
         {
            output.PreElement.SetHtmlContent("<div class='input-group input-group-required'>");
            output.PostElement.SetHtmlContent("<div class=\"input-group-btn\"><span class=\"required\">*</span></div></div>");
         }

         await base.ProcessAsync(context, output);
      }

      #endregion
   }
}