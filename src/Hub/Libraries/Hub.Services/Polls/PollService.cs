﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Polls;
using Hub.Data;
using Hub.Data.Extensions;

namespace Hub.Services.Polls
{
   /// <summary>
   /// Poll service
   /// </summary>
   public partial class PollService : IPollService
   {
      #region Fields

      private readonly IRepository<Poll> _pollRepository;
      private readonly IRepository<PollAnswer> _pollAnswerRepository;
      private readonly IRepository<PollVotingRecord> _pollVotingRecordRepository;

      #endregion

      #region Ctor

      /// <summary>
      /// IoC Ctor
      /// </summary>
      public PollService(
          IRepository<Poll> pollRepository,
          IRepository<PollAnswer> pollAnswerRepository,
          IRepository<PollVotingRecord> pollVotingRecordRepository)
      {
         _pollRepository = pollRepository;
         _pollAnswerRepository = pollAnswerRepository;
         _pollVotingRecordRepository = pollVotingRecordRepository;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Gets a poll
      /// </summary>
      /// <param name="pollId">The poll identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the poll
      /// </returns>
      public virtual async Task<Poll> GetPollByIdAsync(long pollId)
      {
         return await _pollRepository.GetByIdAsync(pollId, cache => default);
      }

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
      public virtual async Task<IPagedList<Poll>> GetPollsAsync(long languageId = 0, bool showHidden = false,
          bool loadShownOnHomepageOnly = false, string systemKeyword = null,
          int pageIndex = 0, int pageSize = int.MaxValue)
      {
         var query = _pollRepository.Table;

         //whether to load not published, not started and expired polls
         if (!showHidden)
         {
            var utcNow = DateTime.UtcNow;
            query = query.Where(poll => poll.Published);
            query = query.Where(poll => !poll.StartDateUtc.HasValue || poll.StartDateUtc <= utcNow);
            query = query.Where(poll => !poll.EndDateUtc.HasValue || poll.EndDateUtc >= utcNow);
         }

         //load homepage polls only
         if (loadShownOnHomepageOnly)
            query = query.Where(poll => poll.ShowOnHomepage);

         //filter by language
         if (languageId > 0)
            query = query.Where(poll => poll.LanguageId == languageId);

         //filter by system keyword
         if (!string.IsNullOrEmpty(systemKeyword))
            query = query.Where(poll => poll.SystemKeyword == systemKeyword);

         //order records by display order
         query = query.OrderBy(poll => poll.DisplayOrder).ThenBy(poll => poll.Id);

         //return paged list of polls
         return await query.ToPagedListAsync(pageIndex, pageSize);
      }

      /// <summary>
      /// Deletes a poll
      /// </summary>
      /// <param name="poll">The poll</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task DeletePollAsync(Poll poll)
      {
         await _pollRepository.DeleteAsync(poll);
      }

      /// <summary>
      /// Inserts a poll
      /// </summary>
      /// <param name="poll">Poll</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task InsertPollAsync(Poll poll)
      {
         await _pollRepository.InsertAsync(poll);
      }

      /// <summary>
      /// Updates the poll
      /// </summary>
      /// <param name="poll">Poll</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task UpdatePollAsync(Poll poll)
      {
         await _pollRepository.UpdateAsync(poll);
      }

      /// <summary>
      /// Gets a poll answer
      /// </summary>
      /// <param name="pollAnswerId">Poll answer identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the poll answer
      /// </returns>
      public virtual async Task<PollAnswer> GetPollAnswerByIdAsync(long pollAnswerId)
      {
         return await _pollAnswerRepository.GetByIdAsync(pollAnswerId, cache => default);
      }

      /// <summary>
      /// Deletes a poll answer
      /// </summary>
      /// <param name="pollAnswer">Poll answer</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task DeletePollAnswerAsync(PollAnswer pollAnswer)
      {
         await _pollAnswerRepository.DeleteAsync(pollAnswer);
      }

      /// <summary>
      /// Gets a poll answers by parent poll
      /// </summary>
      /// <param name="pollId">The poll identifier</param>
      /// <returns>Poll answer</returns>
      /// <param name="pageIndex">Page index</param>
      /// <param name="pageSize">Page size</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task<IPagedList<PollAnswer>> GetPollAnswerByPollAsync(long pollId, int pageIndex = 0, int pageSize = int.MaxValue)
      {
         var query = _pollAnswerRepository.Table.Where(pa => pa.PollId == pollId);

         //order records by display order
         query = query.OrderBy(pa => pa.DisplayOrder).ThenBy(pa => pa.Id);

         //return paged list of polls
         return await query.ToPagedListAsync(pageIndex, pageSize);
      }

      /// <summary>
      /// Inserts a poll answer
      /// </summary>
      /// <param name="pollAnswer">Poll answer</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task InsertPollAnswerAsync(PollAnswer pollAnswer)
      {
         await _pollAnswerRepository.InsertAsync(pollAnswer);
      }

      /// <summary>
      /// Updates the poll answer
      /// </summary>
      /// <param name="pollAnswer">Poll answer</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task UpdatePollAnswerAsync(PollAnswer pollAnswer)
      {
         await _pollAnswerRepository.UpdateAsync(pollAnswer);
      }

      /// <summary>
      /// Gets a value indicating whether user already voted for this poll
      /// </summary>
      /// <param name="pollId">Poll identifier</param>
      /// <param name="userId">User identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the result
      /// </returns>
      public virtual async Task<bool> AlreadyVotedAsync(long pollId, long userId)
      {
         if (pollId == 0 || userId == 0)
            return false;

         var result = await
             (from pa in _pollAnswerRepository.Table
              join pvr in _pollVotingRecordRepository.Table on pa.Id equals pvr.PollAnswerId
              where pa.PollId == pollId && pvr.UserId == userId
              select pvr)
             .AnyAsync();
         return result;
      }

      /// <summary>
      /// Inserts a poll voting record
      /// </summary>
      /// <param name="pollVotingRecord">Voting record</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task InsertPollVotingRecordAsync(PollVotingRecord pollVotingRecord)
      {
         await _pollVotingRecordRepository.InsertAsync(pollVotingRecord);
      }

      /// <summary>
      /// Gets a poll voting records by parent answer
      /// </summary>
      /// <param name="pollAnswerId">Poll answer identifier</param>
      /// <returns>Poll answer</returns>
      /// <param name="pageIndex">Page index</param>
      /// <param name="pageSize">Page size</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      public virtual async Task<IPagedList<PollVotingRecord>> GetPollVotingRecordsByPollAnswerAsync(long pollAnswerId, int pageIndex = 0, int pageSize = int.MaxValue)
      {
         var query = _pollVotingRecordRepository.Table.Where(pa => pa.PollAnswerId == pollAnswerId);

         //return paged list of poll voting records
         return await query.ToPagedListAsync(pageIndex, pageSize);
      }

      #endregion
   }
}