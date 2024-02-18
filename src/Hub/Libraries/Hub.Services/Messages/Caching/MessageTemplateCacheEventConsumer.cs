﻿using Hub.Core.Domain.Messages;
using Hub.Services.Caching;
using System.Threading.Tasks;

namespace Hub.Services.Messages.Caching
{
   /// <summary>
   /// Represents a message template cache event consumer
   /// </summary>
   public partial class MessageTemplateCacheEventConsumer : CacheEventConsumer<MessageTemplate>
   {
      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(MessageTemplate entity)
      {
         await RemoveByPrefixAsync(AppMessageDefaults.MessageTemplatesByNamePrefix, entity.Name);
      }
   }
}
