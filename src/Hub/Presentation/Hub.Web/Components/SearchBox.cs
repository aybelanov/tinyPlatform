using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hub.Web.Factories;
using Hub.Web.Framework.Components;

namespace Hub.Web.Components;

public class SearchBoxViewComponent : AppViewComponent
{

   public SearchBoxViewComponent()
   {
   }

   public async Task<IViewComponentResult> InvokeAsync()
   {
      //TODO refactor
      return await Task.FromResult<IViewComponentResult>(View());
   }
}
