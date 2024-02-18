using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Profile
{
   public partial record PostsModel : BaseAppModel
   {
      public long ForumTopicId { get; set; }
      public string ForumTopicTitle { get; set; }
      public string ForumTopicSlug { get; set; }
      public string ForumPostText { get; set; }
      public string Posted { get; set; }
   }
}