using System;
using System.Collections.Generic;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Forums
{
   /// <summary>
   /// Represents a forum list model
   /// </summary>
   public partial record ForumModel : BaseAppEntityModel
   {
      #region Ctor

      public ForumModel()
      {
         ForumGroups = new List<ForumGroupModel>();
      }

      #endregion

      #region Properties

      [AppResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.ForumGroupId")]
      public long ForumGroupId { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.Name")]
      public string Name { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.Description")]
      public string Description { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.DisplayOrder")]
      public int DisplayOrder { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.CreatedOn")]
      public DateTime CreatedOn { get; set; }

      public List<ForumGroupModel> ForumGroups { get; set; }

      #endregion
   }
}