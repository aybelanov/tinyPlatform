using Hub.Web.Factories;
using Hub.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hub.Web.Components;

public class LanguageSelectorViewComponent : AppViewComponent
{
   private readonly ICommonModelFactory _commonModelFactory;

   public LanguageSelectorViewComponent(ICommonModelFactory commonModelFactory)
   {
      _commonModelFactory = commonModelFactory;
   }

   public async Task<IViewComponentResult> InvokeAsync()
   {
      var model = await _commonModelFactory.PrepareLanguageSelectorModelAsync();

      if (model.AvailableLanguages.Count == 1)
         return Content("");

      return View(model);
   }
}
