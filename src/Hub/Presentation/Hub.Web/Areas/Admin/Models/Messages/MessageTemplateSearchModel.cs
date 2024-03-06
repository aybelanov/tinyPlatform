using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;

namespace Hub.Web.Areas.Admin.Models.Messages
{
   /// <summary>
   /// Represents a message template search model
   /// </summary>
   public partial record MessageTemplateSearchModel : BaseSearchModel
   {
      #region Properties

      [AppResourceDisplayName("Admin.ContentManagement.MessageTemplates.List.SearchKeywords")]
      public string SearchKeywords { get; set; }

      #endregion
   }
}