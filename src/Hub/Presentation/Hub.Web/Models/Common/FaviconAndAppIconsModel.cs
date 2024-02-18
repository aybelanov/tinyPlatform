using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Common;

public partial record FaviconAndAppIconsModel : BaseAppModel
{
   public string HeadCode { get; set; }
}