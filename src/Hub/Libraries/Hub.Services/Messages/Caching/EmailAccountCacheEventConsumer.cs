using Hub.Core.Domain.Messages;
using Hub.Services.Caching;

namespace Hub.Services.Messages.Caching
{
   /// <summary>
   /// Represents an email account cache event consumer
   /// </summary>
   public partial class EmailAccountCacheEventConsumer : CacheEventConsumer<EmailAccount>
   {
   }
}
