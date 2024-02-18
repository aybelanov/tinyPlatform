﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core.Domain.Polls;
using Hub.Data.Extensions;
using Hub.Services.Helpers;
using Hub.Services.Localization;
using Hub.Services.Polls;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Polls;
using Hub.Web.Framework.Models.Extensions;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the poll model factory implementation
/// </summary>
public partial class PollModelFactory : IPollModelFactory
{
   #region Fields

   private readonly IBaseAdminModelFactory _baseAdminModelFactory;
   private readonly IDateTimeHelper _dateTimeHelper;
   private readonly ILanguageService _languageService;
   private readonly IPollService _pollService;

   #endregion

   #region Ctor

   public PollModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
       IDateTimeHelper dateTimeHelper,
       ILanguageService languageService,
       IPollService pollService)
   {
      _baseAdminModelFactory = baseAdminModelFactory;
      _dateTimeHelper = dateTimeHelper;
      _languageService = languageService;
      _pollService = pollService;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Prepare poll answer search model
   /// </summary>
   /// <param name="searchModel">Poll answer search model</param>
   /// <param name="poll">Poll</param>
   /// <returns>Poll answer search model</returns>
   protected virtual PollAnswerSearchModel PreparePollAnswerSearchModel(PollAnswerSearchModel searchModel, Poll poll)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      if (poll == null)
         throw new ArgumentNullException(nameof(poll));

      searchModel.PollId = poll.Id;

      //prepare page parameters
      searchModel.SetGridPageSize();

      return searchModel;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare poll search model
   /// </summary>
   /// <param name="searchModel">Poll search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the poll search model
   /// </returns>
   public virtual Task<PollSearchModel> PreparePollSearchModelAsync(PollSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare page parameters
      searchModel.SetGridPageSize();

      return Task.FromResult(searchModel);
   }

   /// <summary>
   /// Prepare paged poll list model
   /// </summary>
   /// <param name="searchModel">Poll search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the poll list model
   /// </returns>
   public virtual async Task<PollListModel> PreparePollListModelAsync(PollSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //get polls
      var polls = await _pollService.GetPollsAsync(showHidden: true, pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

      //prepare list model
      var model = await new PollListModel().PrepareToGridAsync(searchModel, polls, () =>
      {
         return polls.SelectAwait(async poll =>
            {
               //fill in model values from the entity
               var pollModel = poll.ToModel<PollModel>();

               //convert dates to the user time
               if (poll.StartDateUtc.HasValue)
                  pollModel.StartDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(poll.StartDateUtc.Value, DateTimeKind.Utc);
               if (poll.EndDateUtc.HasValue)
                  pollModel.EndDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(poll.EndDateUtc.Value, DateTimeKind.Utc);

               //fill in additional values (not existing in the entity)
               pollModel.LanguageName = (await _languageService.GetLanguageByIdAsync(poll.LanguageId))?.Name;

               return pollModel;
            });
      });

      return model;
   }

   /// <summary>
   /// Prepare poll model
   /// </summary>
   /// <param name="model">Poll model</param>
   /// <param name="poll">Poll</param>
   /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the poll model
   /// </returns>
   public virtual async Task<PollModel> PreparePollModelAsync(PollModel model, Poll poll, bool excludeProperties = false)
   {
      if (poll != null)
      {
         //fill in model values from the entity
         model ??= poll.ToModel<PollModel>();

         model.StartDateUtc = poll.StartDateUtc;
         model.EndDateUtc = poll.EndDateUtc;

         //prepare nested search model
         PreparePollAnswerSearchModel(model.PollAnswerSearchModel, poll);
      }

      //set default values for the new model
      if (poll == null)
      {
         model.Published = true;
         model.ShowOnHomepage = true;
      }

      //prepare available languages
      await _baseAdminModelFactory.PrepareLanguagesAsync(model.AvailableLanguages, false);

      return model;
   }

   /// <summary>
   /// Prepare paged poll answer list model
   /// </summary>
   /// <param name="searchModel">Poll answer search model</param>
   /// <param name="poll">Poll</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the poll answer list model
   /// </returns>
   public virtual async Task<PollAnswerListModel> PreparePollAnswerListModelAsync(PollAnswerSearchModel searchModel, Poll poll)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      if (poll == null)
         throw new ArgumentNullException(nameof(poll));

      //get poll answers
      var pollAnswers = await _pollService.GetPollAnswerByPollAsync(poll.Id, searchModel.Page - 1, searchModel.PageSize);

      //prepare list model
      var model = new PollAnswerListModel().PrepareToGrid(searchModel, pollAnswers,
          () => pollAnswers.Select(pollAnswer => pollAnswer.ToModel<PollAnswerModel>()));

      return model;
   }

   #endregion
}