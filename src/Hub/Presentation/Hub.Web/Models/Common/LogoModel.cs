using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Common;

public partial record LogoModel : BaseAppModel
{
   public string AppName { get; set; }

   public string LogoPath { get; set; }
}