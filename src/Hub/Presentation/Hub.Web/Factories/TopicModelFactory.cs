using Hub.Core.Domain.Topics;
using Hub.Services.Localization;
using Hub.Services.Security;
using Hub.Services.Seo;
using Hub.Services.Topics;
using Hub.Web.Models.Topics;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Factories;

/// <summary>
/// Represents the topic model factory
/// </summary>
public partial class TopicModelFactory : ITopicModelFactory
{
   #region Fields

   private readonly IAclService _aclService;
   private readonly ILocalizationService _localizationService;
   private readonly ITopicService _topicService;
   private readonly ITopicTemplateService _topicTemplateService;
   private readonly IUrlRecordService _urlRecordService;

   #endregion

   #region Ctor

   public TopicModelFactory(IAclService aclService,
       ILocalizationService localizationService,
       ITopicService topicService,
       ITopicTemplateService topicTemplateService,
       IUrlRecordService urlRecordService)
   {
      _aclService = aclService;
      _localizationService = localizationService;
      _topicService = topicService;
      _topicTemplateService = topicTemplateService;
      _urlRecordService = urlRecordService;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Prepare the topic model
   /// </summary>
   /// <param name="topic">Topic</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the opic model
   /// </returns>
   protected virtual async Task<TopicModel> PrepareTopicModelAsync(Topic topic)
   {
      if (topic == null)
         throw new ArgumentNullException(nameof(topic));

      var model = new TopicModel
      {
         Id = topic.Id,
         SystemName = topic.SystemName,
         IncludeInSitemap = topic.IncludeInSitemap,
         IsPasswordProtected = topic.IsPasswordProtected,
         Title = topic.IsPasswordProtected ? string.Empty : await _localizationService.GetLocalizedAsync(topic, x => x.Title),
         Body = topic.IsPasswordProtected ? string.Empty : await _localizationService.GetLocalizedAsync(topic, x => x.Body),
         MetaKeywords = await _localizationService.GetLocalizedAsync(topic, x => x.MetaKeywords),
         MetaDescription = await _localizationService.GetLocalizedAsync(topic, x => x.MetaDescription),
         MetaTitle = await _localizationService.GetLocalizedAsync(topic, x => x.MetaTitle),
         SeName = await _urlRecordService.GetSeNameAsync(topic),
         TopicTemplateId = topic.TopicTemplateId
      };

      return model;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Get the topic model by topic identifier
   /// </summary>
   /// <param name="topicId">Topic identifier</param>
   /// <param name="showHidden">A value indicating whether to show hidden records</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the opic model
   /// </returns>
   public virtual async Task<TopicModel> PrepareTopicModelByIdAsync(long topicId, bool showHidden = false)
   {
      var topic = await _topicService.GetTopicByIdAsync(topicId);

      if (topic == null)
         return null;

      if (showHidden)
         return await PrepareTopicModelAsync(topic);

      if (!topic.Published ||
          //ACL (access control list)
          !await _aclService.AuthorizeAsync(topic))
         return null;

      return await PrepareTopicModelAsync(topic);
   }

   /// <summary>
   /// Get the topic model by topic system name
   /// </summary>
   /// <param name="systemName">Topic system name</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the opic model
   /// </returns>
   public virtual async Task<TopicModel> PrepareTopicModelBySystemNameAsync(string systemName)
   {
      var topic = await _topicService.GetTopicBySystemNameAsync(systemName);
      if (topic == null)
         return null;

      return await PrepareTopicModelAsync(topic);
   }

   /// <summary>
   /// Get topic template view path
   /// </summary>
   /// <param name="topicTemplateId">Topic template identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the view path
   /// </returns>
   public virtual async Task<string> PrepareTemplateViewPathAsync(long topicTemplateId)
   {
      var template = await _topicTemplateService.GetTopicTemplateByIdAsync(topicTemplateId) ??
                     (await _topicTemplateService.GetAllTopicTemplatesAsync()).FirstOrDefault();

      if (template == null)
         throw new Exception("No default template could be loaded");

      return template.ViewPath;
   }

   #endregion
}