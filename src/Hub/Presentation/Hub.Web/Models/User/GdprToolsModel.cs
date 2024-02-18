using Hub.Web.Framework.Models;

namespace Hub.Web.Models.User
{
   public partial record GdprToolsModel : BaseAppModel
   {
      public string Result { get; set; }
   }
}