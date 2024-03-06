using Hub.Core;
using Hub.Services.Security;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Components;

/// <summary>
/// Represents a view component that displays common statistics
/// </summary>
public class CommonStatisticsViewComponent : AppViewComponent
{
   #region Fields

   private readonly ICommonModelFactory _commonModelFactory;
   private readonly IPermissionService _permissionService;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   public CommonStatisticsViewComponent(ICommonModelFactory commonModelFactory,
       IPermissionService permissionService,
       IWorkContext workContext)
   {
      _commonModelFactory = commonModelFactory;
      _permissionService = permissionService;
      _workContext = workContext;
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
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
         return Content(string.Empty);

      //prepare model
      var model = await _commonModelFactory.PrepareCommonStatisticsModelAsync();

      return View(model);
   }

   #endregion
}