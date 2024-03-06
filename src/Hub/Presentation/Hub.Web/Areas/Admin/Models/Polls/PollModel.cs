using Hub.Web.Framework.Models;
using Hub.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hub.Web.Areas.Admin.Models.Polls
{
   /// <summary>
   /// Represents a poll model
   /// </summary>
   public partial record PollModel : BaseAppEntityModel
   {
      #region Ctor

      public PollModel()
      {
         AvailableLanguages = new List<SelectListItem>();
         PollAnswerSearchModel = new PollAnswerSearchModel();
      }

      #endregion

      #region Properties

      [AppResourceDisplayName("Admin.ContentManagement.Polls.Fields.Language")]
      public long LanguageId { get; set; }

      public IList<SelectListItem> AvailableLanguages { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Polls.Fields.Language")]
      public string LanguageName { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Polls.Fields.Name")]
      public string Name { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Polls.Fields.SystemKeyword")]
      public string SystemKeyword { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Polls.Fields.Published")]
      public bool Published { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Polls.Fields.ShowOnHomepage")]
      public bool ShowOnHomepage { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Polls.Fields.AllowGuestsToVote")]
      public bool AllowGuestsToVote { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Polls.Fields.DisplayOrder")]
      public int DisplayOrder { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Polls.Fields.StartDate")]
      [UIHint("DateTimeNullable")]
      public DateTime? StartDateUtc { get; set; }

      [AppResourceDisplayName("Admin.ContentManagement.Polls.Fields.EndDate")]
      [UIHint("DateTimeNullable")]
      public DateTime? EndDateUtc { get; set; }

      public PollAnswerSearchModel PollAnswerSearchModel { get; set; }

      #endregion
   }
}