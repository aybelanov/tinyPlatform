using Hub.Web.Framework.Models;

namespace Hub.Web.Models.User
{
   public partial record UserAvatarModel : BaseAppModel
   {
      public string AvatarUrl { get; set; }
   }
}