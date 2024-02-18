using Hub.Web.Framework.Models;

namespace Hub.Web.Areas.Admin.Models.Polls
{
   /// <summary>
   /// Represents a poll answer search model
   /// </summary>
   public partial record PollAnswerSearchModel : BaseSearchModel
   {
      #region Ctor

      public PollAnswerSearchModel()
      {
         AddPollAnswer = new PollAnswerModel();
      }

      #endregion

      #region Properties

      public long PollId { get; set; }

      public PollAnswerModel AddPollAnswer { get; set; }

      #endregion
   }
}