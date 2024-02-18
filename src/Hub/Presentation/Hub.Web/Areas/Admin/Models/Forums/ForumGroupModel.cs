using System;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Forums;

/// <summary>
/// Represents a forum group model
/// </summary>
public partial record ForumGroupModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.ContentManagement.Forums.ForumGroup.Fields.Name")]
   public string Name { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Forums.ForumGroup.Fields.DisplayOrder")]
   public int DisplayOrder { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Forums.ForumGroup.Fields.CreatedOn")]
   public DateTime CreatedOn { get; set; }

   #endregion
}