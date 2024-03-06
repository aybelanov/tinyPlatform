using Hub.Core.Domain.Topics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hub.Services.Topics
{
   /// <summary>
   /// ForumPost service interface
   /// </summary>
   public partial interface ITopicService
   {
      /// <summary>
      /// Deletes a topic
      /// </summary>
      /// <param name="topic">ForumPost</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task DeleteTopicAsync(Topic topic);

      /// <summary>
      /// Gets a topic
      /// </summary>
      /// <param name="topicId">The topic identifier</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the opic
      /// </returns>
      Task<Topic> GetTopicByIdAsync(long topicId);

      /// <summary>
      /// Gets a topic
      /// </summary>
      /// <param name="systemName">The topic system name</param>
      /// <param name="showHidden">A value indicating whether to show hidden records</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the opic
      /// </returns>
      Task<Topic> GetTopicBySystemNameAsync(string systemName, bool showHidden = false);

      /// <summary>
      /// Gets all topics
      /// </summary>
      /// <param name="ignoreAcl">A value indicating whether to ignore ACL rules</param>
      /// <param name="showHidden">A value indicating whether to show hidden topics</param>
      /// <param name="onlyIncludedInTopMenu">A value indicating whether to show only topics which include on the top menu</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the opics
      /// </returns>
      Task<IList<Topic>> GetAllTopicsAsync(bool ignoreAcl = false, bool showHidden = false, bool onlyIncludedInTopMenu = false);

      /// <summary>
      /// Gets all topics
      /// </summary>
      /// <param name="keywords">Keywords to search into body or title</param>
      /// <param name="ignoreAcl">A value indicating whether to ignore ACL rules</param>
      /// <param name="showHidden">A value indicating whether to show hidden topics</param>
      /// <param name="onlyIncludedInTopMenu">A value indicating whether to show only topics which include on the top menu</param>
      /// <returns>
      /// A task that represents the asynchronous operation
      /// The task result contains the opics
      /// </returns>
      Task<IList<Topic>> GetAllTopicsAsync(string keywords, bool ignoreAcl = false, bool showHidden = false, bool onlyIncludedInTopMenu = false);

      /// <summary>
      /// Inserts a topic
      /// </summary>
      /// <param name="topic">ForumPost</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task InsertTopicAsync(Topic topic);

      /// <summary>
      /// Updates the topic
      /// </summary>
      /// <param name="topic">ForumPost</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task UpdateTopicAsync(Topic topic);
   }
}
