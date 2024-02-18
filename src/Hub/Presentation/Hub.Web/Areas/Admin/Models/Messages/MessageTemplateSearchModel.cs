using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

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