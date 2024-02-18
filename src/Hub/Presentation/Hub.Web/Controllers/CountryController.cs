using System.Threading.Tasks;
using Hub.Web.Factories;
using Microsoft.AspNetCore.Mvc;
using Hub.Web.Framework.Mvc.Filters;

namespace Hub.Web.Controllers;

public partial class CountryController : BasePublicController
{
   #region Fields

   private readonly ICountryModelFactory _countryModelFactory;

   #endregion

   #region Ctor

   public CountryController(ICountryModelFactory countryModelFactory)
   {
      _countryModelFactory = countryModelFactory;
   }

   #endregion

   #region States / provinces

   //available even when navigation is not allowed
   [CheckAccessPublicPlatform(true)]
   //ignore SEO friendly URLs checks
   [CheckLanguageSeoCode(true)]
   public virtual async Task<IActionResult> GetStatesByCountryId(string countryId, bool addSelectStateItem)
   {
      var model = await _countryModelFactory.GetStatesByCountryIdAsync(countryId, addSelectStateItem);

      return Json(model);
   }

   #endregion
}