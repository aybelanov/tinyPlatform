using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hub.Core.Domain.Users;
using Hub.Web.Factories;
using Hub.Web.Framework.Components;

namespace Hub.Web.Components;

public class NewsletterBoxViewComponent : AppViewComponent
{
   private readonly UserSettings _userSettings;
   private readonly INewsletterModelFactory _newsletterModelFactory;

   public NewsletterBoxViewComponent(UserSettings userSettings, INewsletterModelFactory newsletterModelFactory)
   {
      _userSettings = userSettings;
      _newsletterModelFactory = newsletterModelFactory;
   }

   public async Task<IViewComponentResult> InvokeAsync()
   {
      if (_userSettings.HideNewsletterBlock)
         return Content("");

      var model = await _newsletterModelFactory.PrepareNewsletterBoxModelAsync();
      return View(model);
   }
}
