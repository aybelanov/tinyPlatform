using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Common;

public partial record SocialModel : BaseAppModel
{
   public string FacebookLink { get; set; }
   public string TwitterLink { get; set; }
   public string YoutubeLink { get; set; }
   public long WorkingLanguageId { get; set; }
   public bool NewsEnabled { get; set; }
}