using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.News
{
   /// <summary>
   /// Represents a news item search model
   /// </summary>
   public partial record NewsItemSearchModel : BaseSearchModel
   {
      #region Properties

      public string SearchTitle { get; set; }

      #endregion
   }
}