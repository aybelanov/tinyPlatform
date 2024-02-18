using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Logging
{
   /// <summary>
   /// Represents a log search model
   /// </summary>
   public partial record LogSearchModel : BaseSearchModel
   {
      #region Ctor

      public LogSearchModel()
      {
         AvailableLogLevels = new List<SelectListItem>();
      }

      #endregion

      #region Properties

      [AppResourceDisplayName("Admin.System.Log.List.CreatedOnFrom")]
      [UIHint("DateNullable")]
      public DateTime? CreatedOnFrom { get; set; }

      [AppResourceDisplayName("Admin.System.Log.List.CreatedOnTo")]
      [UIHint("DateNullable")]
      public DateTime? CreatedOnTo { get; set; }

      [AppResourceDisplayName("Admin.System.Log.List.Message")]
      public string Message { get; set; }

      [AppResourceDisplayName("Admin.System.Log.List.LogLevel")]
      public int LogLevelId { get; set; }

      public IList<SelectListItem> AvailableLogLevels { get; set; }

      #endregion
   }
}