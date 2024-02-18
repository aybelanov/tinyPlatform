using System.Collections.Generic;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Common
{
   public partial record HubThemeSelectorModel : BaseAppModel
   {
      public HubThemeSelectorModel()
      {
         AvailableHubThemes = new List<HubThemeModel>();
      }

      public IList<HubThemeModel> AvailableHubThemes { get; set; }

      public HubThemeModel CurrentHubTheme { get; set; }
   }
}