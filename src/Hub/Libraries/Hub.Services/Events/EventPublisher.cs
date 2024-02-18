﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core.Events;
using Hub.Core.Infrastructure;
using Hub.Services.Logging;

namespace Hub.Services.Events;

/// <summary>
/// Represents the event publisher implementation
/// </summary>
public partial class EventPublisher : IEventPublisher
{
   #region Methods

   /// <summary>
   /// Publish event to consumers
   /// </summary>
   /// <typeparam name="TEvent">Type of event</typeparam>
   /// <param name="event">Event object</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task PublishAsync<TEvent>(TEvent @event)
   {
      //get all event consumers
      var consumers = EngineContext.Current.ResolveAll<IConsumer<TEvent>>().ToList();

      foreach (var consumer in consumers)
      {
         try
         {
            //try to handle published event
            await consumer.HandleEventAsync(@event);
         }
         catch (Exception exception)
         {
            //log error, we put in to nested try-catch to prevent possible cyclic (if some error occurs)
            try
            {
               var logger = EngineContext.Current.Resolve<ILogger>();
               if (logger == null)
                  return;

               await logger.ErrorAsync(exception.Message, exception);
            }
            catch
            {
               // ignored
            }
         }
      }
   }

   #endregion
}