using Hub.Core;
using Hub.Services.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class PreferencesController : BaseAdminController
{
   #region Fields

   private readonly IGenericAttributeService _genericAttributeService;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   public PreferencesController(IGenericAttributeService genericAttributeService,
       IWorkContext workContext)
   {
      _genericAttributeService = genericAttributeService;
      _workContext = workContext;
   }

   #endregion

   #region Methods

   [HttpPost]
   public virtual async Task<IActionResult> SavePreference(string name, bool value)
   {
      //permission validation is not required here
      if (string.IsNullOrEmpty(name))
         throw new ArgumentNullException(nameof(name));

      await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentUserAsync(), name, value);

      return Json(new
      {
         Result = true
      });
   }

   #endregion
}