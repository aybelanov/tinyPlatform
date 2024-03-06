﻿using Hub.Core;
using Hub.Web.Framework.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace Hub.Web.Framework.TagHelpers.Admin
{
   /// <summary>
   /// "app-nested-setting" tag helper
   /// </summary>
   [HtmlTargetElement("app-nested-setting", Attributes = FOR_ATTRIBUTE_NAME)]
   public class AppNestedSettingTagHelper : TagHelper
   {
      #region Constants

      private const string FOR_ATTRIBUTE_NAME = "asp-for";
      private const string IS_CONDITION_INVERT = "is-condition-invert";
      private const string DISABLE_AUTOGENERATION = "disable-auto-generation";

      #endregion

      #region Properties

      /// <summary> Html generator </summary>
      protected IHtmlGenerator Generator { get; set; }

      /// <summary>
      /// An expression to be evaluated against the current model
      /// </summary>
      [HtmlAttributeName(FOR_ATTRIBUTE_NAME)]
      public ModelExpression For { get; set; }

      /// <summary>
      /// Is condition inverted
      /// </summary>
      [HtmlAttributeName(IS_CONDITION_INVERT)]
      public bool IsConditionInvert { get; set; }

      /// <summary>
      /// Disable auto-generation js script
      /// </summary>
      [HtmlAttributeName(DISABLE_AUTOGENERATION)]
      public bool DisableAutoGeneration { get; set; }

      /// <summary>
      /// ViewContext
      /// </summary>
      [HtmlAttributeNotBound]
      [ViewContext]
      public ViewContext ViewContext { get; set; }

      #endregion

      #region Ctor

      /// <summary> IoC Ctor </summary>
      public AppNestedSettingTagHelper(IHtmlGenerator generator)
      {
         Generator = generator;
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

         var parentSettingName = For.Name;
         var jsConsistentParentSettingName = parentSettingName.Replace('.', '_');

         var random = CommonHelper.GenerateRandomInteger();
         var nestedSettingId = $"nestedSetting{random}";
         var parentSettingId = $"parentSetting{random}";

         //tag details
         output.TagName = "div";
         output.TagMode = TagMode.StartTagAndEndTag;
         output.Attributes.Add("class", "nested-setting");

         if (context.AllAttributes.ContainsName("id"))
            nestedSettingId = context.AllAttributes["id"].Value.ToString();
         output.Attributes.Add("id", nestedSettingId);

         //use javascript
         var script = new TagBuilder("script");

         var isNot = IsConditionInvert ? "!" : "";

         script.InnerHtml.AppendHtml(
             "$(document).ready(function () {" +
                 $"initNestedSetting('{parentSettingName}', '{parentSettingId}', '{nestedSettingId}');"
         );

         if (!DisableAutoGeneration)
            script.InnerHtml.AppendHtml(
                $"$('#{jsConsistentParentSettingName}').click(toggle_{jsConsistentParentSettingName});" +
                $"toggle_{jsConsistentParentSettingName}();"
            );

         script.InnerHtml.AppendHtml("});");

         if (!DisableAutoGeneration)
            script.InnerHtml.AppendHtml(
                $"function toggle_{jsConsistentParentSettingName}() " + "{" +
                    $"if ({isNot}$('#{jsConsistentParentSettingName}').is(':checked')) " + "{" +
                        $"$('#{nestedSettingId}').showElement();" +
                    "} else {" +
                        $"$('#{nestedSettingId}').hideElement();" +
                    "}" +
                "}"
            );

         var scriptTag = await script.RenderHtmlContentAsync();
         output.PreContent.SetHtmlContent(scriptTag);
      }

      #endregion
   }
}