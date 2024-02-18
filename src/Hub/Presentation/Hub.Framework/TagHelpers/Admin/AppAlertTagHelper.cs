﻿using System;
using System.Threading.Tasks;
using Hub.Web.Framework.Extensions;
using Hub.Web.Framework.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Hub.Web.Framework.TagHelpers.Admin
{
   /// <summary>
   /// "app-alert" tag helper
   /// </summary>
   [HtmlTargetElement("app-alert", Attributes = ALERT_NAME_ID, TagStructure = TagStructure.WithoutEndTag)]
   public class AppAlertTagHelper : TagHelper
   {
      #region Constants

      private const string ALERT_NAME_ID = "asp-alert-id";
      private const string ALERT_MESSAGE_NAME = "asp-alert-message";

      #endregion

      #region Properties

      /// <summary> Html generator </summary>
      protected IHtmlGenerator Generator { get; set; }

      /// <summary>
      /// Alert identifier
      /// </summary>
      [HtmlAttributeName(ALERT_NAME_ID)]
      public string AlertId { get; set; }

      /// <summary>
      /// Additional confirm text
      /// </summary>
      [HtmlAttributeName(ALERT_MESSAGE_NAME)]
      public string Message { get; set; }

      /// <summary>
      /// ViewContext
      /// </summary>
      [HtmlAttributeNotBound]
      [ViewContext]
      public ViewContext ViewContext { get; set; }

      #endregion

      #region Fields

      private readonly IHtmlHelper _htmlHelper;

      #endregion

      #region Ctor

      /// <summary> Default Ctor </summary>
      public AppAlertTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper)
      {
         Generator = generator;
         _htmlHelper = htmlHelper;
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

         //contextualize IHtmlHelper
         var viewContextAware = _htmlHelper as IViewContextAware;
         viewContextAware?.Contextualize(ViewContext);

         var modalId = await new HtmlString(AlertId + "-action-alert").RenderHtmlContentAsync();

         var actionAlertModel = new ActionAlertModel()
         {
            AlertId = AlertId,
            WindowId = modalId,
            AlertMessage = Message
         };

         //tag details
         output.TagName = "div";
         output.TagMode = TagMode.StartTagAndEndTag;

         output.Attributes.Add("id", modalId);
         output.Attributes.Add("class", "modal fade");
         output.Attributes.Add("tabindex", "-1");
         output.Attributes.Add("role", "dialog");
         output.Attributes.Add("aria-labelledby", $"{modalId}-title");

         var partialView = await _htmlHelper.PartialAsync("Alert", actionAlertModel);
         output.Content.SetHtmlContent(partialView);

         //modal script
         var script = new TagBuilder("script");
         script.InnerHtml.AppendHtml(
             "$(document).ready(function () {" +
                 $"$('#{AlertId}').attr(\"data-toggle\", \"modal\").attr(\"data-target\", \"#{modalId}\")" +
             "});");
         var scriptTag = await script.RenderHtmlContentAsync();
         output.PostContent.SetHtmlContent(scriptTag);
      }

      #endregion
   }
}