using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Areas.Admin.Models.News
{
   /// <summary>
   /// Represents a news comment search model
   /// </summary>
   public partial record NewsCommentSearchModel : BaseSearchModel
   {
      #region Ctor

      public NewsCommentSearchModel()
      {
         AvailableApprovedOptions = new List<SelectListItem>();
      }

      #endregion

      #region Properties

      public long? NewsItemId { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.News.Comments.List.CreatedOnFrom")]
      [UIHint("DateNullable")]
      public DateTime? CreatedOnFrom { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.News.Comments.List.CreatedOnTo")]
      [UIHint("DateNullable")]
      public DateTime? CreatedOnTo { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.News.Comments.List.SearchText")]
      public string SearchText { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.News.Comments.List.SearchApproved")]
      public int SearchApprovedId { get; set; }

      public IList<SelectListItem> AvailableApprovedOptions { get; set; }

      #endregion
   }
}