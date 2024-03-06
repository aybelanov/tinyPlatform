using Hub.Web.Factories;
using Hub.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hub.Web.Components;

public class LogoViewComponent : AppViewComponent
{
   private readonly ICommonModelFactory _commonModelFactory;

   public LogoViewComponent(ICommonModelFactory commonModelFactory)
   {
      _commonModelFactory = commonModelFactory;
   }

   public async Task<IViewComponentResult> InvokeAsync()
   {
      var model = await _commonModelFactory.PrepareLogoModelAsync();
      return View(model);
   }
}
