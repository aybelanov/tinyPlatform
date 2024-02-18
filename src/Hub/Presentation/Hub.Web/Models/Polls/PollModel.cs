using System.Collections.Generic;
using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Polls
{
   public partial record PollModel : BaseAppEntityModel
   {
      public PollModel()
      {
         Answers = new List<PollAnswerModel>();
      }

      public string Name { get; set; }

      public bool AlreadyVoted { get; set; }

      public int TotalVotes { get; set; }

      public IList<PollAnswerModel> Answers { get; set; }
   }

   public partial record PollAnswerModel : BaseAppEntityModel
   {
      public string Name { get; set; }

      public int NumberOfVotes { get; set; }

      public double PercentOfTotalVotes { get; set; }
   }
}