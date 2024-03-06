using Hub.Core;
using Hub.Core.Domain;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Localization;
using Hub.Core.Domain.Security;
using Hub.Core.Domain.Users;
using Hub.Services.Common;
using Hub.Services.Directory;
using Hub.Services.Html;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Web.Factories;
using Hub.Web.Framework.Mvc.Filters;
using Hub.Web.Framework.Themes;
using Hub.Web.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Threading.Tasks;

namespace Hub.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class CommonController : BasePublicController
{
   #region Fields

   private readonly CaptchaSettings _captchaSettings;
   private readonly CommonSettings _commonSettings;
   private readonly ICommonModelFactory _commonModelFactory;
   private readonly ICurrencyService _currencyService;
   private readonly IUserActivityService _userActivityService;
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly IHtmlFormatter _htmlFormatter;
   private readonly ILanguageService _languageService;
   private readonly ILocalizationService _localizationService;
   private readonly IThemeContext _themeContext;
   private readonly IWorkContext _workContext;
   private readonly IWorkflowMessageService _workflowMessageService;
   private readonly LocalizationSettings _localizationSettings;
   private readonly SitemapSettings _sitemapSettings;
   private readonly SitemapXmlSettings _sitemapXmlSettings;
   private readonly AppInfoSettings _appInformationSettings;

   #endregion

   #region Ctor

   public CommonController(CaptchaSettings captchaSettings,
       CommonSettings commonSettings,
       ICommonModelFactory commonModelFactory,
       ICurrencyService currencyService,
       IUserActivityService userActivityService,
       IGenericAttributeService genericAttributeService,
       IHtmlFormatter htmlFormatter,
       ILanguageService languageService,
       ILocalizationService localizationService,
       IThemeContext themeContext,
       IWorkContext workContext,
       IWorkflowMessageService workflowMessageService,
       LocalizationSettings localizationSettings,
       SitemapSettings sitemapSettings,
       SitemapXmlSettings sitemapXmlSettings,
       AppInfoSettings appInformationSettings)
   {
      _captchaSettings = captchaSettings;
      _commonSettings = commonSettings;
      _commonModelFactory = commonModelFactory;
      _currencyService = currencyService;
      _userActivityService = userActivityService;
      _genericAttributeService = genericAttributeService;
      _htmlFormatter = htmlFormatter;
      _languageService = languageService;
      _localizationService = localizationService;
      _themeContext = themeContext;
      _workContext = workContext;
      _workflowMessageService = workflowMessageService;
      _localizationSettings = localizationSettings;
      _sitemapSettings = sitemapSettings;
      _sitemapXmlSettings = sitemapXmlSettings;
      _appInformationSettings = appInformationSettings;
   }

   #endregion

   #region Methods

   //page not found
   public virtual IActionResult PageNotFound()
   {
      Response.StatusCode = 404;
      Response.ContentType = "text/html";

      return View();
   }

   //available even when a platform is closed
   [CheckAccessClosedPlatform(true)]
   //available even when navigation is not allowed
   [CheckAccessPublicPlatform(true)]
   public virtual async Task<IActionResult> SetLanguage(long langid, string returnUrl = "")
   {
      var language = await _languageService.GetLanguageByIdAsync(langid);
      if (!language?.Published ?? false)
         language = await _workContext.GetWorkingLanguageAsync();

      //home page
      if (string.IsNullOrEmpty(returnUrl))
         returnUrl = Url.RouteUrl("Homepage");

      //language part in URL
      if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
      {
         //remove current language code if it's already localized URL
         if ((await returnUrl.IsLocalizedUrlAsync(Request.PathBase, true)).IsLocalized)
            returnUrl = returnUrl.RemoveLanguageSeoCodeFromUrl(Request.PathBase, true);

         //and add code of passed language
         returnUrl = returnUrl.AddLanguageSeoCodeToUrl(Request.PathBase, true, language);
      }

      await _workContext.SetWorkingLanguageAsync(language);

      //prevent open redirection attack
      if (!Url.IsLocalUrl(returnUrl))
         returnUrl = Url.RouteUrl("Homepage");

      return Redirect(returnUrl);
   }

   //available even when navigation is not allowed
   [CheckAccessPublicPlatform(true)]
   public virtual async Task<IActionResult> SetCurrency(long userCurrencyId, string returnUrl = "")
   {
      var currency = await _currencyService.GetCurrencyByIdAsync(userCurrencyId);
      if (currency != null)
         await _workContext.SetWorkingCurrencyAsync(currency);

      //home page
      if (string.IsNullOrEmpty(returnUrl))
         returnUrl = Url.RouteUrl("Homepage");

      //prevent open redirection attack
      if (!Url.IsLocalUrl(returnUrl))
         returnUrl = Url.RouteUrl("Homepage");

      return Redirect(returnUrl);
   }


   //contact us page
   //available even when a platform is closed
   [CheckAccessPublicPlatform(true)]
   [EnableRateLimiting("form")]
   public virtual async Task<IActionResult> ContactUs()
   {
      var model = new ContactUsModel();
      model = await _commonModelFactory.PrepareContactUsModelAsync(model, false);

      return View(model);
   }

   [HttpPost, ActionName("ContactUs")]
   [ValidateCaptcha]
   //available even when a platform is closed
   [CheckAccessPublicPlatform(true)]
   [EnableRateLimiting("form")]
   public virtual async Task<IActionResult> ContactUsSend(ContactUsModel model, bool captchaValid)
   {
      //validate CAPTCHA
      if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
         ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

      model = await _commonModelFactory.PrepareContactUsModelAsync(model, true);

      if (ModelState.IsValid)
      {
         var subject = _commonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;
         var body = _htmlFormatter.FormatText(model.Enquiry, false, true, false, false, false, false);

         await _workflowMessageService.SendContactUsMessageAsync((await _workContext.GetWorkingLanguageAsync()).Id,
             model.Email.Trim(), model.FullName, subject, body);

         model.SuccessfullySent = true;
         model.Result = await _localizationService.GetResourceAsync("ContactUs.YourEnquiryHasBeenSent");

         //activity log
         await _userActivityService.InsertActivityAsync("PublicPlatform.ContactUs",
             await _localizationService.GetResourceAsync("ActivityLog.PublicPlatform.ContactUs"));

         return View(model);
      }

      return View(model);
   }


   //sitemap page
   public virtual async Task<IActionResult> Sitemap(SitemapPageModel pageModel)
   {
      if (!_sitemapSettings.SitemapEnabled)
         return RedirectToRoute("Homepage");

      var model = await _commonModelFactory.PrepareSitemapModelAsync(pageModel);

      return View(model);
   }

   //SEO sitemap page
   //available even when a platform is closed
   [CheckAccessPublicPlatform(true)]
   //ignore SEO friendly URLs checks
   [CheckLanguageSeoCode(true)]
   public virtual async Task<IActionResult> SitemapXml(int? id)
   {
      var siteMap = _sitemapXmlSettings.SitemapXmlEnabled
          ? await _commonModelFactory.PrepareSitemapXmlAsync(id) : string.Empty;

      return Content(siteMap, "text/xml");
   }

   public virtual async Task<IActionResult> SetPlatformTheme(string themeName, string returnUrl = "")
   {
      await _themeContext.SetWorkingThemeNameAsync(themeName);

      //home page
      if (string.IsNullOrEmpty(returnUrl))
         returnUrl = Url.RouteUrl("Homepage");

      //prevent open redirection attack
      if (!Url.IsLocalUrl(returnUrl))
         returnUrl = Url.RouteUrl("Homepage");

      return Redirect(returnUrl);
   }

   [HttpPost]
   //available even when a platform is closed
   [CheckAccessPublicPlatform(true)]
   //available even when navigation is not allowed
   [CheckAccessPublicPlatform(true)]
   public virtual async Task<IActionResult> EuCookieLawAccept()
   {
      if (!_appInformationSettings.DisplayEuCookieLawWarning)
         //disabled
         return Json(new { stored = false });

      //save setting
      await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentUserAsync(), AppUserDefaults.EuCookieLawAcceptedAttribute, true);
      return Json(new { stored = true });
   }

   //robots.txt file
   //available even when a platform is closed
   [CheckAccessPublicPlatform(true)]
   //available even when navigation is not allowed
   [CheckAccessPublicPlatform(true)]
   //ignore SEO friendly URLs checks
   [CheckLanguageSeoCode(true)]
   public virtual async Task<IActionResult> RobotsTextFile()
   {
      var robotsFileContent = await _commonModelFactory.PrepareRobotsTextFileAsync();

      return Content(robotsFileContent, MimeTypes.TextPlain);
   }

   public virtual IActionResult GenericUrl()
   {
      //seems that no entity was found
      return InvokeHttp404();
   }

   //platform is closed
   //available even when a platform is closed
   [CheckAccessPublicPlatform(true)]
   public virtual IActionResult PlatformClosed()
   {
      return View();
   }

   //helper method to redirect users. Workaround for GenericPathRoute class where we're not allowed to do it
   public virtual IActionResult InternalRedirect(string url, bool permanentRedirect)
   {
      //ensure it's invoked from our GenericPathRoute class
      if (HttpContext.Items["hub.RedirectFromGenericPathRoute"] == null ||
          !Convert.ToBoolean(HttpContext.Items["hub.RedirectFromGenericPathRoute"]))
      {
         url = Url.RouteUrl("Homepage");
         permanentRedirect = false;
      }

      //home page
      if (string.IsNullOrEmpty(url))
      {
         url = Url.RouteUrl("Homepage");
         permanentRedirect = false;
      }

      //prevent open redirection attack
      if (!Url.IsLocalUrl(url))
      {
         url = Url.RouteUrl("Homepage");
         permanentRedirect = false;
      }

      if (permanentRedirect)
         return RedirectPermanent(url);

      return Redirect(url);
   }

   #endregion
}