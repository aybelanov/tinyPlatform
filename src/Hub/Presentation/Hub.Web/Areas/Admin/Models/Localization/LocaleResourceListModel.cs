using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Localization
{
   /// <summary>
   /// Represents a locale resource list model
   /// </summary>
   public record LocaleResourceListModel : BasePagedListModel<LocaleResourceModel>
   {
   }
}