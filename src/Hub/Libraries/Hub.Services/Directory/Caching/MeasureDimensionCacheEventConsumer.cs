using System.Threading.Tasks;
using Hub.Core.Domain.Directory;
using Hub.Services.Caching;

namespace Hub.Services.Directory.Caching
{
   /// <summary>
   /// Represents a measure dimension cache event consumer
   /// </summary>
   public partial class MeasureDimensionCacheEventConsumer : CacheEventConsumer<MeasureDimension>
   {
   }
}
