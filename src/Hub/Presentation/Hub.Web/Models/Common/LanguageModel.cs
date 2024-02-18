using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Common;

public partial record LanguageModel : BaseAppEntityModel
{
   public string Name { get; set; }

   public string FlagImageFileName { get; set; }
}