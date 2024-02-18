using System;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Topics;
using Hub.Data.Extensions;
using Hub.Services.Localization;
using Hub.Services.Seo;
using Hub.Services.Topics;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Topics;
using Hub.Web.Framework.Factories;
using Hub.Web.Framework.Models.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the topic model factory implementation
/// </summary>
public partial class TopicModelFactory : ITopicModelFactory
{
   #region Fields

   private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
   private readonly IActionContextAccessor _actionContextAccessor;
   private readonly IBaseAdminModelFactory _baseAdminModelFactory;
   private readonly ILocalizationService _localizationService;
   private readonly ILocalizedModelFactory _localizedModelFactory;
   private readonly ITopicService _topicService;
   private readonly IUrlHelperFactory _urlHelperFactory;
   private readonly IUrlRecordService _urlRecordService;
   private readonly IWebHelper _webHelper;

   #endregion

   #region Ctor

   public TopicModelFactory(IAclSupportedModelFactory aclSupportedModelFactory,
       IActionContextAccessor actionContextAccessor,
       IBaseAdminModelFactory baseAdminModelFactory,
       ILocalizationService localizationService,
       ILocalizedModelFactory localizedModelFactory,
       ITopicService topicService,
       IUrlHelperFactory urlHelperFactory,
       IUrlRecordService urlRecordService,
       IWebHelper webHelper)
   {
      _aclSupportedModelFactory = aclSupportedModelFactory;
      _actionContextAccessor = actionContextAccessor;
      _baseAdminModelFactory = baseAdminModelFactory;
      _localizationService = localizationService;
      _localizedModelFactory = localizedModelFactory;
      _topicService = topicService;
      _urlHelperFactory = urlHelperFactory;
      _urlRecordService = urlRecordService;
      _webHelper = webHelper;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare topic search model
   /// </summary>
   /// <param name="searchModel">Topic search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the opic search model
   /// </returns>
   public virtual Task<TopicSearchModel> PrepareTopicSearchModelAsync(TopicSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare page parameters
      searchModel.SetGridPageSize();

      return Task.FromResult(searchModel);
   }

   /// <summary>
   /// Prepare paged topic list model
   /// </summary>
   /// <param name="searchModel">Topic search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the opic list model
   /// </returns>
   public virtual async Task<TopicListModel> PrepareTopicListModelAsync(TopicSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //get topics
      var topics = await _topicService.GetAllTopicsAsync(showHidden: true,
          keywords: searchModel.SearchKeywords,
          ignoreAcl: true);

      var pagedTopics = topics.ToPagedList(searchModel);

      //prepare grid model
      var model = await new TopicListModel().PrepareToGridAsync(searchModel, pagedTopics, () =>
      {
         return pagedTopics.SelectAwait(async topic =>
            {
               //fill in model values from the entity
               var topicModel = topic.ToModel<TopicModel>();

               //little performance optimization: ensure that "Body" is not returned
               topicModel.Body = string.Empty;

               topicModel.SeName = await _urlRecordService.GetSeNameAsync(topic, 0, true, false);

               if (!string.IsNullOrEmpty(topicModel.SystemName))
                  topicModel.TopicName = topicModel.SystemName;
               else
                  topicModel.TopicName = topicModel.Title;

               return topicModel;
            });
      });

      return model;
   }

   /// <summary>
   /// Prepare topic model
   /// </summary>
   /// <param name="model">Topic model</param>
   /// <param name="topic">Topic</param>
   /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the opic model
   /// </returns>
   public virtual async Task<TopicModel> PrepareTopicModelAsync(TopicModel model, Topic topic, bool excludeProperties = false)
   {
      Func<TopicLocalizedModel, long, Task> localizedModelConfiguration = null;

      if (topic != null)
      {
         //fill in model values from the entity
         if (model == null)
         {
            model = topic.ToModel<TopicModel>();
            model.SeName = await _urlRecordService.GetSeNameAsync(topic, 0, true, false);
         }

         model.Url = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext)
             .RouteUrl("Topic", new { SeName = await _urlRecordService.GetSeNameAsync(topic) }, _webHelper.GetCurrentRequestProtocol());

         //define localized model configuration action
         localizedModelConfiguration = async (locale, languageId) =>
         {
            locale.Title = await _localizationService.GetLocalizedAsync(topic, entity => entity.Title, languageId, false, false);
            locale.Body = await _localizationService.GetLocalizedAsync(topic, entity => entity.Body, languageId, false, false);
            locale.MetaKeywords = await _localizationService.GetLocalizedAsync(topic, entity => entity.MetaKeywords, languageId, false, false);
            locale.MetaDescription = await _localizationService.GetLocalizedAsync(topic, entity => entity.MetaDescription, languageId, false, false);
            locale.MetaTitle = await _localizationService.GetLocalizedAsync(topic, entity => entity.MetaTitle, languageId, false, false);
            locale.SeName = await _urlRecordService.GetSeNameAsync(topic, languageId, false, false);
         };
      }

      //set default values for the new model
      if (topic == null)
      {
         model.DisplayOrder = 1;
         model.Published = true;
      }

      //prepare localized models
      if (!excludeProperties)
         model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

      //prepare available topic templates
      await _baseAdminModelFactory.PrepareTopicTemplatesAsync(model.AvailableTopicTemplates, false);

      //prepare model user roles
      await _aclSupportedModelFactory.PrepareModelUserRolesAsync(model, topic, excludeProperties);

      return model;
   }

   #endregion
}