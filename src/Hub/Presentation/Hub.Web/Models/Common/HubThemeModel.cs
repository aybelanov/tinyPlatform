using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Common
{
   public partial record HubThemeModel : BaseAppModel
   {
      public string Name { get; set; }
      public string Title { get; set; }
   }
}