using System.Threading.Tasks;
using Hub.Web.Areas.Admin.Factories;
using Microsoft.AspNetCore.Mvc;
using Hub.Web.Framework.Components;

namespace Hub.Web.Areas.Admin.Components
{
   /// <summary>
   /// Represents a view component that displays the admin language selector
   /// </summary>
   public class AdminLanguageSelectorViewComponent : AppViewComponent
   {
      #region Fields

      private readonly ICommonModelFactory _commonModelFactory;

      #endregion

      #region Ctor

      public AdminLanguageSelectorViewComponent(ICommonModelFactory commonModelFactory)
      {
         _commonModelFactory = commonModelFactory;
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
         //prepare model
         var model = await _commonModelFactory.PrepareLanguageSelectorModelAsync();

         return View(model);
      }

      #endregion
   }
}