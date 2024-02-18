using Hub.Core.Domain.Media;
using Hub.Services.Caching;

namespace Hub.Services.Media.Caching
{
   /// <summary>
   /// Represents a picture binary cache event consumer
   /// </summary>
   public partial class PictureBinaryCacheEventConsumer : CacheEventConsumer<PictureBinary>
   {
   }
}
