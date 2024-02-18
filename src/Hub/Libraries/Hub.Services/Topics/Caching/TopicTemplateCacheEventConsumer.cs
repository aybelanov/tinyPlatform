using System.Threading.Tasks;
using Hub.Core.Domain.Topics;
using Hub.Services.Caching;

namespace Hub.Services.Topics.Caching
{
    /// <summary>
    /// Represents a topic template cache event consumer
    /// </summary>
    public partial class TopicTemplateCacheEventConsumer : CacheEventConsumer<TopicTemplate>
    {
    }
}
