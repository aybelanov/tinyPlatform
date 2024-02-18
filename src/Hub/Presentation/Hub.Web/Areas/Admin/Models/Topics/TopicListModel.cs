using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Topics
{
   /// <summary>
   /// Represents a topic list model
   /// </summary>
   public partial record TopicListModel : BasePagedListModel<TopicModel>
   {
   }
}