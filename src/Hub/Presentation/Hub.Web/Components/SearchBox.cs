using Hub.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
