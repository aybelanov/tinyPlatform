using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hub.Web.Factories;
using Hub.Web.Framework.Components;

namespace Hub.Web.Components;

public class TopMenuViewComponent : AppViewComponent
{
   private readonly ICommonModelFactory _commonModelFactory;

   public TopMenuViewComponent(ICommonModelFactory commonModelFactory)
   {
      _commonModelFactory = commonModelFactory;
   }

   public async Task<IViewComponentResult> InvokeAsync(int? thumbPictureSize)
   {
      var model = await _commonModelFactory.PrepareTopMenuModelAsync();
      return View(model);
   }
}
