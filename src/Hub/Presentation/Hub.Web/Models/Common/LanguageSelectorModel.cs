using System.Collections.Generic;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Common;

public partial record LanguageSelectorModel : BaseAppModel
{
   public LanguageSelectorModel()
   {
      AvailableLanguages = new List<LanguageModel>();
   }

   public IList<LanguageModel> AvailableLanguages { get; set; }

   public long CurrentLanguageId { get; set; }

   public bool UseImages { get; set; }
}