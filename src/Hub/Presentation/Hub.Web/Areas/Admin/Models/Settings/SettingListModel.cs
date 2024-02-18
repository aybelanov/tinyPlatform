using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Settings
{
   /// <summary>
   /// Represents a setting list model
   /// </summary>
   public partial record SettingListModel : BasePagedListModel<SettingModel>
   {
   }
}