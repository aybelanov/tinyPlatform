using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Polls;

/// <summary>
/// Represents a poll answer model
/// </summary>
public partial record PollAnswerModel : BaseAppEntityModel
{
   #region Properties

   public long PollId { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Polls.Answers.Fields.Name")]
   public string Name { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Polls.Answers.Fields.NumberOfVotes")]
   public int NumberOfVotes { get; set; }

   [AppResourceDisplayName("Admin.ContentManagement.Polls.Answers.Fields.DisplayOrder")]
   public int DisplayOrder { get; set; }

   #endregion
}