using Hub.Core.Domain.Forums;
using Hub.Web.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Hub.Web.Models.Boards;

public partial record EditForumTopicModel : BaseAppModel
{
   #region Ctor

   public EditForumTopicModel()
   {
      TopicPriorities = new List<SelectListItem>();
   }

   #endregion

   #region Properties

   public bool IsEdit { get; set; }

   public long Id { get; set; }

   public long ForumId { get; set; }

   public string ForumName { get; set; }

   public string ForumSeName { get; set; }

   public long TopicTypeId { get; set; }

   public EditorType ForumEditor { get; set; }

   public string Subject { get; set; }

   public string Text { get; set; }

   public bool IsUserAllowedToSetTopicPriority { get; set; }

   public IEnumerable<SelectListItem> TopicPriorities { get; set; }

   public bool IsUserAllowedToSubscribe { get; set; }

   public bool Subscribed { get; set; }

   public bool DisplayCaptcha { get; set; }

   #endregion
}