using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Common;

public partial record AdminHeaderLinksModel : BaseAppModel
{
   public string ImpersonatedUserName { get; set; }
   public bool IsUserImpersonated { get; set; }
   public bool DisplayAdminLink { get; set; }
   public string EditPageUrl { get; set; }
}