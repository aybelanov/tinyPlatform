using System.Collections.Generic;
using Hub.Web.Areas.Admin.Models.Localization;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents an admin language selector model
/// </summary>
public partial record LanguageSelectorModel : BaseAppModel
{
   #region Ctor

   public LanguageSelectorModel()
   {
      AvailableLanguages = new List<LanguageModel>();
   }

   #endregion

   #region Properties

   public IList<LanguageModel> AvailableLanguages { get; set; }

   public LanguageModel CurrentLanguage { get; set; }

   #endregion
}