using Hub.Core.Domain.Topics;
using Hub.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Services.Topics;

/// <summary>
/// ForumPost template service
/// </summary>
public partial class TopicTemplateService : ITopicTemplateService
{
   #region Fields

   private readonly IRepository<TopicTemplate> _topicTemplateRepository;

   #endregion

   #region Ctor

   /// <summary>
   /// IoC Ctor
   /// </summary>
   /// <param name="topicTemplateRepository"></param>
   public TopicTemplateService(IRepository<TopicTemplate> topicTemplateRepository)
   {
      _topicTemplateRepository = topicTemplateRepository;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Delete topic template
   /// </summary>
   /// <param name="topicTemplate">ForumPost template</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task DeleteTopicTemplateAsync(TopicTemplate topicTemplate)
   {
      await _topicTemplateRepository.DeleteAsync(topicTemplate);
   }

   /// <summary>
   /// Gets all topic templates
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the opic templates
   /// </returns>
   public virtual async Task<IList<TopicTemplate>> GetAllTopicTemplatesAsync()
   {
      var templates = await _topicTemplateRepository.GetAllAsync(query =>
      {
         return from pt in query
                orderby pt.DisplayOrder, pt.Id
                select pt;
      }, cache => default);

      return templates;
   }

   /// <summary>
   /// Gets a topic template
   /// </summary>
   /// <param name="topicTemplateId">ForumPost template identifier</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the opic template
   /// </returns>
   public virtual async Task<TopicTemplate> GetTopicTemplateByIdAsync(long topicTemplateId)
   {
      return await _topicTemplateRepository.GetByIdAsync(topicTemplateId, cache => default);
   }

   /// <summary>
   /// Inserts topic template
   /// </summary>
   /// <param name="topicTemplate">ForumPost template</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task InsertTopicTemplateAsync(TopicTemplate topicTemplate)
   {
      await _topicTemplateRepository.InsertAsync(topicTemplate);
   }

   /// <summary>
   /// Updates the topic template
   /// </summary>
   /// <param name="topicTemplate">ForumPost template</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task UpdateTopicTemplateAsync(TopicTemplate topicTemplate)
   {
      await _topicTemplateRepository.UpdateAsync(topicTemplate);
   }

   #endregion
}