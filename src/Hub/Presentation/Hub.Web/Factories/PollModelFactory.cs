using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Web.Models.Polls;
using Hub.Core;
using Hub.Core.Caching;
using Hub.Core.Domain.Polls;
using Hub.Data.Extensions;
using Hub.Services.Polls;
using Hub.Web.Infrastructure.Cache;

namespace Hub.Web.Factories
{
   /// <summary>
   /// Represents the poll model factory
   /// </summary>
   public partial class PollModelFactory : IPollModelFactory
   {
      #region Fields

      private readonly IPollService _pollService;
      private readonly IStaticCacheManager _staticCacheManager;
      private readonly IWorkContext _workContext;

      #endregion

      #region Ctor

      public PollModelFactory(IPollService pollService,
          IStaticCacheManager staticCacheManager,
          
          IWorkContext workContext)
      {
         _pollService = pollService;
         _staticCacheManager = staticCacheManager;
         _workContext = workContext;
      }

      #endregion

      #region Methods

      /// <summary>
      /// Prepare the poll model
      /// </summary>
      /// <param name="poll">Poll</param>
      /// <param name="setAlreadyVotedProperty">Whether to load a value indicating that user already voted for this poll</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the poll model
      /// </returns>
      public virtual async Task<PollModel> PreparePollModelAsync(Poll poll, bool setAlreadyVotedProperty)
      {
         if (poll == null)
            throw new ArgumentNullException(nameof(poll));

         var user = await _workContext.GetCurrentUserAsync();

         var model = new PollModel
         {
            Id = poll.Id,
            AlreadyVoted = setAlreadyVotedProperty && await _pollService.AlreadyVotedAsync(poll.Id, user.Id),
            Name = poll.Name
         };
         var answers = await _pollService.GetPollAnswerByPollAsync(poll.Id);

         foreach (var answer in answers)
            model.TotalVotes += answer.NumberOfVotes;
         foreach (var pa in answers)
            model.Answers.Add(new PollAnswerModel
            {
               Id = pa.Id,
               Name = pa.Name,
               NumberOfVotes = pa.NumberOfVotes,
               PercentOfTotalVotes = model.TotalVotes > 0 ? Convert.ToDouble(pa.NumberOfVotes) / Convert.ToDouble(model.TotalVotes) * Convert.ToDouble(100) : 0,
            });

         return model;
      }

      /// <summary>
      /// Get the poll model by poll system keyword
      /// </summary>
      /// <param name="systemKeyword">Poll system keyword</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the poll model
      /// </returns>
      public virtual async Task<PollModel> PreparePollModelBySystemNameAsync(string systemKeyword)
      {
         if (string.IsNullOrWhiteSpace(systemKeyword))
            return null;

         var currentLanguage = await _workContext.GetWorkingLanguageAsync();
         var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(AppModelCacheDefaults.PollBySystemNameModelKey, systemKeyword, currentLanguage);

         var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
         {
            var poll = (await _pollService
                   .GetPollsAsync(currentLanguage.Id, systemKeyword: systemKeyword))
                   .FirstOrDefault();

            //we do not cache nulls. that's why let's return an empty record (ID = 0)
            if (poll == null)
               return new PollModel { Id = 0 };

            return await PreparePollModelAsync(poll, false);
         });

         if ((cachedModel?.Id ?? 0) == 0)
            return null;

         //"AlreadyVoted" property of "PollModel" object depends on the current user. Let's update it.
         //But first we need to clone the cached model (the updated one should not be cached)
         var model = cachedModel with { };
         var user = await _workContext.GetCurrentUserAsync();
         model.AlreadyVoted = await _pollService.AlreadyVotedAsync(model.Id, user.Id);

         return model;
      }

      /// <summary>
      /// Prepare the home page poll models
      /// </summary>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the list of the poll model
      /// </returns>
      public virtual async Task<List<PollModel>> PrepareHomepagePollModelsAsync()
      {
         var user = await _workContext.GetCurrentUserAsync();
         var language = await _workContext.GetWorkingLanguageAsync();
         var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(AppModelCacheDefaults.HomepagePollsModelKey, language);

         var cachedPolls = await _staticCacheManager.GetAsync(cacheKey, async () =>
         {
            var polls = await _pollService.GetPollsAsync(language.Id, loadShownOnHomepageOnly: true);
            var pollsModels = await polls.SelectAwait(async poll => await PreparePollModelAsync(poll, false)).ToListAsync();
            return pollsModels;
         });

         //"AlreadyVoted" property of "PollModel" object depends on the current user. Let's update it.
         //But first we need to clone the cached model (the updated one should not be cached)
         var model = new List<PollModel>();
         foreach (var poll in cachedPolls)
         {
            var pollModel = poll with { };
            pollModel.AlreadyVoted = await _pollService.AlreadyVotedAsync(pollModel.Id, user.Id);
            model.Add(pollModel);
         }

         return model;
      }

      #endregion
   }
}
