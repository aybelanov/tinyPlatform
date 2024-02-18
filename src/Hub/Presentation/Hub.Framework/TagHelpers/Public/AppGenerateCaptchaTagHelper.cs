using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Hub.Core.Domain.Security;
using Hub.Web.Framework.Security.Captcha;

namespace Hub.Web.Framework.TagHelpers.Public
{
   /// <summary>
   /// "app-captcha" tag helper
   /// </summary>
   [HtmlTargetElement("app-captcha", TagStructure = TagStructure.WithoutEndTag)]
   public class AppGenerateCaptchaTagHelper : TagHelper
   {
      #region Properties

      /// <summary>
      /// ViewContext
      /// </summary>
      [HtmlAttributeNotBound]
      [ViewContext]
      public ViewContext ViewContext { get; set; }

      #endregion

      #region Fields

      private readonly CaptchaSettings _captchaSettings;
      private readonly IHtmlHelper _htmlHelper;

      #endregion

      #region Ctor

      /// <summary> IoC Ctor </summary>
      public AppGenerateCaptchaTagHelper(CaptchaSettings captchaSettings, IHtmlHelper htmlHelper)
      {
         _captchaSettings = captchaSettings;
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

         IHtmlContent captchaHtmlContent;
         switch (_captchaSettings.CaptchaType)
         {
            case CaptchaType.CheckBoxReCaptchaV2:
               output.Attributes.Add("class", "captcha-box");
               captchaHtmlContent = await _htmlHelper.GenerateCheckBoxReCaptchaV2Async(_captchaSettings);
               break;
            case CaptchaType.ReCaptchaV3:
               captchaHtmlContent = await _htmlHelper.GenerateReCaptchaV3Async(_captchaSettings);
               break;
            default:
               throw new InvalidOperationException("Invalid captcha type.");
         }

         //tag details
         output.TagName = "div";
         output.TagMode = TagMode.StartTagAndEndTag;
         output.Content.SetHtmlContent(captchaHtmlContent);
      }

      #endregion
   }
}