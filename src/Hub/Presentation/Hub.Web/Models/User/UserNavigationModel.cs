using System.Collections.Generic;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.User;

public partial record UserNavigationModel : BaseAppModel
{
   public UserNavigationModel()
   {
      UserNavigationItems = new List<UserNavigationItemModel>();
   }

   public IList<UserNavigationItemModel> UserNavigationItems { get; set; }

   public int SelectedTab { get; set; }
}

public record UserNavigationItemModel : BaseAppModel
{
   public string RouteName { get; set; }
   public string Title { get; set; }
   public int Tab { get; set; }
   public string ItemClass { get; set; }
}

public enum UserNavigationEnum
{
   Info = 0,
   Addresses = 10,
   ChangePassword = 70,
   Avatar = 80,
   ForumSubscriptions = 90,
   GdprTools = 120,
   MultiFactorAuthentication = 140
}