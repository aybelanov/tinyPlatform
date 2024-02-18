using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Core.Domain.Topics;

namespace Hub.Services.Topics
{
    /// <summary>
    /// ForumPost template service interface
    /// </summary>
    public partial interface ITopicTemplateService
    {
        /// <summary>
        /// Delete topic template
        /// </summary>
        /// <param name="topicTemplate">ForumPost template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteTopicTemplateAsync(TopicTemplate topicTemplate);

        /// <summary>
        /// Gets all topic templates
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opic templates
        /// </returns>
        Task<IList<TopicTemplate>> GetAllTopicTemplatesAsync();

        /// <summary>
        /// Gets a topic template
        /// </summary>
        /// <param name="topicTemplateId">ForumPost template identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opic template
        /// </returns>
        Task<TopicTemplate> GetTopicTemplateByIdAsync(long topicTemplateId);

        /// <summary>
        /// Inserts topic template
        /// </summary>
        /// <param name="topicTemplate">ForumPost template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertTopicTemplateAsync(TopicTemplate topicTemplate);

        /// <summary>
        /// Updates the topic template
        /// </summary>
        /// <param name="topicTemplate">ForumPost template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateTopicTemplateAsync(TopicTemplate topicTemplate);
    }
}
