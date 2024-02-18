using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Templates;

/// <summary>
/// Represents a templates model
/// </summary>
public partial record TemplatesModel : BaseAppModel
{
   #region Ctor

   public TemplatesModel()
   {
      TemplatesTopic = new TopicTemplateSearchModel();
      AddTopicTemplate = new TopicTemplateModel();
   }

   #endregion

   #region Properties

   public TopicTemplateSearchModel TemplatesTopic { get; set; }

   public TopicTemplateModel AddTopicTemplate { get; set; }

   #endregion
}
