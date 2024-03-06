using Hub.Core.Domain;
using Hub.Web.Factories;
using Hub.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hub.Web.Components
{
   public class HubThemeSelectorViewComponent : AppViewComponent
   {
      private readonly ICommonModelFactory _commonModelFactory;
      private readonly AppInfoSettings _appInformationSettings;

      public HubThemeSelectorViewComponent(ICommonModelFactory commonModelFactory,
          AppInfoSettings appInformationSettings)
      {
         _commonModelFactory = commonModelFactory;
         _appInformationSettings = appInformationSettings;
      }

      public async Task<IViewComponentResult> InvokeAsync()
      {
         if (!_appInformationSettings.AllowUserToSelectTheme)
            return Content("");

         var model = await _commonModelFactory.PreparePlatformThemeSelectorModelAsync();
         return View(model);
      }
   }
}
