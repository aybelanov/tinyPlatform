using System.Threading.Tasks;
using Hub.Core.Domain.Topics;
using Hub.Services.Caching;

namespace Hub.Services.Topics.Caching
{
    /// <summary>
    /// Represents a topic cache event consumer
    /// </summary>
    public partial class TopicCacheEventConsumer : CacheEventConsumer<Topic>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Topic entity)
        {
            await RemoveByPrefixAsync(AppTopicDefaults.TopicBySystemNamePrefix, entity.SystemName);
        }
    }
}
