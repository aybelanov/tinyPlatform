using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Hub.Services.Configuration;

using Hub.Web.Framework.Components;
using System;

namespace Hub.Web.Areas.Admin.Components
{
   public class AclDisabledWarningViewComponent : AppViewComponent
   {
      
      private readonly ISettingService _settingService;
      

      public AclDisabledWarningViewComponent(ISettingService settingService)
      {
         _settingService = settingService;
      }

      public Task<IViewComponentResult> InvokeAsync()
      {

         // TODO refactor
         ////This setting is disabled. No warnings.
         //if (!enabled)
         return Task.FromResult<IViewComponentResult>(Content(string.Empty));

         //return View();

         //throw new NotImplementedException();
      }
   }
}
