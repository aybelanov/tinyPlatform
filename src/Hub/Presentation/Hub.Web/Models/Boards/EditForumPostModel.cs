using Hub.Core.Domain.Forums;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Boards;

public partial record EditForumPostModel : BaseAppModel
{
   #region Properties

   public long Id { get; set; }

   public long ForumTopicId { get; set; }

   public bool IsEdit { get; set; }

   public string Text { get; set; }

   public EditorType ForumEditor { get; set; }

   public string ForumName { get; set; }

   public string ForumTopicSubject { get; set; }

   public string ForumTopicSeName { get; set; }

   public bool IsUserAllowedToSubscribe { get; set; }

   public bool Subscribed { get; set; }

   public bool DisplayCaptcha { get; set; }

   #endregion
}