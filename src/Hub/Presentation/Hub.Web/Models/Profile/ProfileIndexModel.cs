using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Profile
{
   public partial record ProfileIndexModel : BaseAppModel
   {
      public long UserProfileId { get; set; }
      public string ProfileTitle { get; set; }
      public int PostsPage { get; set; }
      public bool PagingPosts { get; set; }
      public bool ForumsEnabled { get; set; }
   }
}