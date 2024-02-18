using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hub.Web.Factories;
using Hub.Web.Framework.Components;

namespace Hub.Web.Components;

public class FaviconViewComponent : AppViewComponent
{
   private readonly ICommonModelFactory _commonModelFactory;

   public FaviconViewComponent(ICommonModelFactory commonModelFactory)
   {
      _commonModelFactory = commonModelFactory;
   }

   public async Task<IViewComponentResult> InvokeAsync()
   {
      var model = await _commonModelFactory.PrepareFaviconAndAppIconsModelAsync();
      if (string.IsNullOrEmpty(model.HeadCode))
         return Content("");
      return View(model);
   }
}
