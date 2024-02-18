using System;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Services.Security;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Areas.Admin.Controllers;

public class SearchCompleteController : BaseAdminController
{
   #region Fields

   private readonly IPermissionService _permissionService;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   public SearchCompleteController(
       IPermissionService permissionService,
       IWorkContext workContext)
   {
      _permissionService = permissionService;
      _workContext = workContext;
   }

   #endregion

   #region Methods

   public virtual Task<IActionResult> SearchAutoComplete(string term)
   {
      throw new NotImplementedException();

      //if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
      //   return Content(string.Empty);

      //const int searchTermMinimumLength = 3;
      //if (string.IsNullOrWhiteSpace(term) || term.Length < searchTermMinimumLength)
      //   return Content(string.Empty);

      ////a vendor should have access only to his products
      //var currentVendor = await _workContext.GetCurrentVendorAsync();
      //var vendorId = 0;
      //if (currentVendor != null)
      //   vendorId = currentVendor.Id;

      ////products
      //const int productNumber = 15;
      //var products = await _productService.SearchProductsAsync(0,
      //    vendorId: vendorId,
      //    keywords: term,
      //    pageSize: productNumber,
      //    showHidden: true);

      //var result = (from p in products
      //              select new
      //              {
      //                 label = p.Name,
      //                 productid = p.Id
      //              }).ToList();

      //return Json(result);
   }

   #endregion
}