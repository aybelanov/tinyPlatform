﻿using Hub.Core.Domain.Common;
using Hub.Services.Caching;
using System.Threading.Tasks;

namespace Hub.Services.Common.Caching
{
   /// <summary>
   /// Represents a address attribute cache event consumer
   /// </summary>
   public partial class AddressAttributeCacheEventConsumer : CacheEventConsumer<AddressAttribute>
   {
      /// <summary>
      /// Clear cache data
      /// </summary>
      /// <param name="entity">Entity</param>
      /// <returns>A task that represents the asynchronous operation</returns>
      protected override async Task ClearCacheAsync(AddressAttribute entity)
      {
         await RemoveAsync(HubCommonDefaults.AddressAttributeValuesByAttributeCacheKey, entity);
      }
   }
}
