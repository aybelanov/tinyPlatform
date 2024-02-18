using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Templates
{
   /// <summary>
   /// Represents a topic template list model
   /// </summary>
   public partial record TopicTemplateListModel : BasePagedListModel<TopicTemplateModel>
   {
   }
}