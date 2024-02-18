using System.Threading.Tasks;
using Hub.Web.Areas.Admin.Factories;
using Microsoft.AspNetCore.Mvc;
using Hub.Web.Framework.Components;

namespace Hub.Web.Areas.Admin.Components
{
   /// <summary>
   /// Represents a view component that displays the service news
   /// </summary>
   public class IotNewsViewComponent : AppViewComponent
   {
      #region Fields

      private readonly IHomeModelFactory _homeModelFactory;

      #endregion

      #region Ctor

      public IotNewsViewComponent(IHomeModelFactory homeModelFactory)
      {
         _homeModelFactory = homeModelFactory;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Invoke view component
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the view component result
      /// </returns>
      public async Task<IViewComponentResult> InvokeAsync()
      {
         try
         {
            //prepare model
            var model = await _homeModelFactory.PrepareTinyPlatformNewsModelAsync();

            return View(model);
         }
         catch
         {
            return Content(string.Empty);
         }
      }

      #endregion
   }
}