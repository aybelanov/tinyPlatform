using Hub.Core.Domain.Users;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Common;

public partial record HeaderLinksModel : BaseAppModel
{
   public bool IsAuthenticated { get; set; }
   public bool HasToClientAcccess { get; set; }
   public string UserName { get; set; }
   public bool AllowPrivateMessages { get; set; }
   public string UnreadPrivateMessages { get; set; }
   public string AlertMessage { get; set; }
   public UserRegistrationType RegistrationType { get; set; }
   public bool WishlistEnabled { get; set; }
   public int WishlistItems { get; set; }

}