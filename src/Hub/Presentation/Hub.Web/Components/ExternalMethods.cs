using Hub.Web.Factories;
using Hub.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hub.Web.Components;

public class ExternalMethodsViewComponent : AppViewComponent
{
   #region Fields

   private readonly IExternalAuthenticationModelFactory _externalAuthenticationModelFactory;

   #endregion

   #region Ctor

   public ExternalMethodsViewComponent(IExternalAuthenticationModelFactory externalAuthenticationModelFactory)
   {
      _externalAuthenticationModelFactory = externalAuthenticationModelFactory;
   }

   #endregion

   #region Methods

   public async Task<IViewComponentResult> InvokeAsync()
   {
      var model = await _externalAuthenticationModelFactory.PrepareExternalMethodsModelAsync();

      return View(model);
   }

   #endregion
}
