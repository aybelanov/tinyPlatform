using Hub.Core;
using Hub.Core.Domain;
using Hub.Core.Domain.Users;
using Hub.Core.Http;
using Hub.Services.Common;
using Hub.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hub.Web.Components;

public class EuCookieLawViewComponent : AppViewComponent
{
   private readonly IGenericAttributeService _genericAttributeService;
   private readonly IWorkContext _workContext;
   private readonly AppInfoSettings _appInformationSettings;

   public EuCookieLawViewComponent(IGenericAttributeService genericAttributeService,

       IWorkContext workContext,
       AppInfoSettings appInformationSettings)
   {
      _genericAttributeService = genericAttributeService;
      _workContext = workContext;
      _appInformationSettings = appInformationSettings;
   }

   public async Task<IViewComponentResult> InvokeAsync()
   {
      if (!_appInformationSettings.DisplayEuCookieLawWarning)
         //disabled
         return Content("");

      //ignore search engines because some pages could be indexed with the EU cookie as description
      var user = await _workContext.GetCurrentUserAsync();
      if (user.IsSearchEngineAccount())
         return Content("");


      if (await _genericAttributeService.GetAttributeAsync<bool>(user, AppUserDefaults.EuCookieLawAcceptedAttribute))
         //already accepted
         return Content("");

      //ignore notification?
      //right now it's used during logout so popup window is not displayed twice
      if (TempData[$"{AppCookieDefaults.Prefix}{AppCookieDefaults.IgnoreEuCookieLawWarning}"] != null && Convert.ToBoolean(TempData[$"{AppCookieDefaults.Prefix}{AppCookieDefaults.IgnoreEuCookieLawWarning}"]))
         return Content("");

      return View();
   }
}