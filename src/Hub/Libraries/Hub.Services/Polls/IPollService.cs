using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Polls;

namespace Hub.Services.Polls
{
   /// <summary>
   /// Poll service interface
   /// </summary>
   public partial interface IPollService
   {
      /// <summary>
      /// Gets a poll
      /// </summary>
      /// <param name="pollId">The poll identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the poll
      /// </returns>
      Task<Poll> GetPollByIdAsync(long pollId);

      /// <summary>
      /// Gets polls
      /// </summary>
      /// <param name="languageId">Language identifier; pass 0 to load all records</param>
      /// <param name="showHidden">Whether to show hidden records (not published, not started and expired)</param>
      /// <param name="loadShownOnHomepageOnly">Retrieve only shown on home page polls</param>
      /// <param name="systemKeyword">The poll system keyword; pass null to load all records</param>
      /// <param name="pageIndex">Page index</param>
      /// <param name="pageSize">Page size</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the polls
      /// </returns>
      Task<IPagedList<Poll>> GetPollsAsync(long languageId = 0, bool showHidden = false,
          bool loadShownOnHomepageOnly = false, string systemKeyword = null,
          int pageIndex = 0, int pageSize = int.MaxValue);

      /// <summary>
      /// Deletes a poll
      /// </summary>
      /// <param name="poll">The poll</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeletePollAsync(Poll poll);

      /// <summary>
      /// Inserts a poll
      /// </summary>
      /// <param name="poll">Poll</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task InsertPollAsync(Poll poll);

      /// <summary>
      /// Updates the poll
      /// </summary>
      /// <param name="poll">Poll</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task UpdatePollAsync(Poll poll);

      /// <summary>
      /// Gets a poll answer
      /// </summary>
      /// <param name="pollAnswerId">Poll answer identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the poll answer
      /// </returns>
      Task<PollAnswer> GetPollAnswerByIdAsync(long pollAnswerId);

      /// <summary>
      /// Gets a poll answers by parent poll
      /// </summary>
      /// <param name="pollId">The poll identifier</param>
      /// <returns>Poll answer</returns>
      /// <param name="pageIndex">Page index</param>
      /// <param name="pageSize">Page size</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task<IPagedList<PollAnswer>> GetPollAnswerByPollAsync(long pollId, int pageIndex = 0, int pageSize = int.MaxValue);

      /// <summary>
      /// Deletes a poll answer
      /// </summary>
      /// <param name="pollAnswer">Poll answer</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeletePollAnswerAsync(PollAnswer pollAnswer);

      /// <summary>
      /// Inserts a poll answer
      /// </summary>
      /// <param name="pollAnswer">Poll answer</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task InsertPollAnswerAsync(PollAnswer pollAnswer);

      /// <summary>
      /// Updates the poll answer
      /// </summary>
      /// <param name="pollAnswer">Poll answer</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task UpdatePollAnswerAsync(PollAnswer pollAnswer);

      /// <summary>
      /// Gets a value indicating whether user already voted for this poll
      /// </summary>
      /// <param name="pollId">Poll identifier</param>
      /// <param name="userId">User identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the result
      /// </returns>
      Task<bool> AlreadyVotedAsync(long pollId, long userId);

      /// <summary>
      /// Inserts a poll voting record
      /// </summary>
      /// <param name="pollVotingRecord">Voting record</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task InsertPollVotingRecordAsync(PollVotingRecord pollVotingRecord);

      /// <summary>
      /// Gets a poll voting records by parent answer
      /// </summary>
      /// <param name="pollAnswerId">Poll answer identifier</param>
      /// <returns>Poll answer</returns>
      /// <param name="pageIndex">Page index</param>
      /// <param name="pageSize">Page size</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task<IPagedList<PollVotingRecord>> GetPollVotingRecordsByPollAnswerAsync(long pollAnswerId, int pageIndex = 0, int pageSize = int.MaxValue);
   }
}