using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Templates;

/// <summary>
/// Represents a topic template model
/// </summary>
public partial record TopicTemplateModel : BaseAppEntityModel
{
   #region Properties

   [AppResourceDisplayName("Admin.System.Templates.Topic.Name")]
   public string Name { get; set; }

   [AppResourceDisplayName("Admin.System.Templates.Topic.ViewPath")]
   public string ViewPath { get; set; }

   [AppResourceDisplayName("Admin.System.Templates.Topic.DisplayOrder")]
   public int DisplayOrder { get; set; }

   #endregion
}