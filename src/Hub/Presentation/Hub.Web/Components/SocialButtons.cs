using Hub.Web.Factories;
using Hub.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hub.Web.Components
{
   public class SocialButtonsViewComponent : AppViewComponent
   {
      private readonly ICommonModelFactory _commonModelFactory;

      public SocialButtonsViewComponent(ICommonModelFactory commonModelFactory)
      {
         _commonModelFactory = commonModelFactory;
      }

      public async Task<IViewComponentResult> InvokeAsync()
      {
         var model = await _commonModelFactory.PrepareSocialModelAsync();
         return View(model);
      }
   }
}
