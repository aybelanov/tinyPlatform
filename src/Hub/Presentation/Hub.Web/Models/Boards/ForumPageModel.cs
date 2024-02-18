using System.Collections.Generic;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Boards;

public partial record ForumPageModel : BaseAppModel
{
   public ForumPageModel()
   {
      ForumTopics = new List<ForumTopicRowModel>();
   }

   public long Id { get; set; }
   public string Name { get; set; }
   public string SeName { get; set; }
   public string Description { get; set; }

   public string WatchForumText { get; set; }

   public IList<ForumTopicRowModel> ForumTopics { get; set; }
   public int TopicPageSize { get; set; }
   public int TopicTotalRecords { get; set; }
   public int TopicPageIndex { get; set; }

   public bool IsUserAllowedToSubscribe { get; set; }

   public bool ForumFeedsEnabled { get; set; }

   public int PostsPageSize { get; set; }

   public bool AllowPostVoting { get; set; }
}