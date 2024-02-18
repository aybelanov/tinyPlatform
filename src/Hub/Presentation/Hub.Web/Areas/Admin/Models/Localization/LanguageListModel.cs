using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Localization
{
   /// <summary>
   /// Represents a language list model
   /// </summary>
   public partial record LanguageListModel : BasePagedListModel<LanguageModel>
   {
   }
}