using Hub.Web.Framework.Models;
using Hub.Web.Models.Common;
using System.Collections.Generic;

namespace Hub.Web.Models.Boards;

public partial record UserForumSubscriptionsModel : BaseAppModel
{
   public UserForumSubscriptionsModel()
   {
      ForumSubscriptions = new List<ForumSubscriptionModel>();
   }

   public IList<ForumSubscriptionModel> ForumSubscriptions { get; set; }
   public PagerModel PagerModel { get; set; }

   #region Nested classes

   public partial record ForumSubscriptionModel : BaseAppEntityModel
   {
      public long ForumId { get; set; }
      public long ForumTopicId { get; set; }
      public bool TopicSubscription { get; set; }
      public string Title { get; set; }
      public string Slug { get; set; }
   }

   #endregion
}