using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Common;

public partial record SystemWarningModel : BaseAppModel
{
   public SystemWarningLevel Level { get; set; }

   public string Text { get; set; }

   public bool DontEncode { get; set; }
}