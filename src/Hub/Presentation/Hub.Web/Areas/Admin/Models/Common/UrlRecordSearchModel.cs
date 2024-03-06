using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Hub.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents an URL record search model
/// </summary>
public partial record UrlRecordSearchModel : BaseSearchModel
{
   #region Ctor

   public UrlRecordSearchModel()
   {
      AvailableLanguages = new List<SelectListItem>();
      AvailableActiveOptions = new List<SelectListItem>();
   }

   #endregion

   #region Properties

   [AppResourceDisplayName("Admin.System.SeNames.List.Name")]
   public string SeName { get; set; }

   [AppResourceDisplayName("Admin.System.SeNames.List.Language")]
   public long LanguageId { get; set; }

   [AppResourceDisplayName("Admin.System.SeNames.List.IsActive")]
   public int IsActiveId { get; set; }

   public IList<SelectListItem> AvailableLanguages { get; set; }

   public IList<SelectListItem> AvailableActiveOptions { get; set; }

   #endregion
}