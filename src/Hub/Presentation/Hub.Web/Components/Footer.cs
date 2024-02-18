using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hub.Web.Factories;
using Hub.Web.Framework.Components;

namespace Hub.Web.Components;

public class FooterViewComponent : AppViewComponent
{
   private readonly ICommonModelFactory _commonModelFactory;

   public FooterViewComponent(ICommonModelFactory commonModelFactory)
   {
      _commonModelFactory = commonModelFactory;
   }

   public async Task<IViewComponentResult> InvokeAsync()
   {
      var model = await _commonModelFactory.PrepareFooterModelAsync();
      return View(model);
   }
}
