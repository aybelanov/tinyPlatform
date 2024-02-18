using Shared.Devices.Proto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devices.Dispatcher.Services;

/// <summary>
/// Represents a point-to-point service implementation
/// </summary>
public class Point2PointService : IPoint2PointService
{
   #region fields

   private TaskCompletionSource<Func<ClientMsg>> taskCompletion;
   private Queue<Func<ClientMsg>> queue;
   private readonly object sync = new();
   private bool isStopping;

   #endregion

   #region Ctors

   /// <summary>
   /// Default Ctor
   /// </summary>
   public Point2PointService()
   {
      queue = new();
   }

   #endregion

   /// <inheritdoc/>
   public void AddServerNotification(ClientMsg msg) => AddServerNotification(() => msg);

   /// <inheritdoc/>
   public void AddServerNotification(Func<ClientMsg> notification)
   {
      lock (sync)
         if (!taskCompletion?.TrySetResult(notification) ?? true)
            queue.Enqueue(notification);
   }

   /// <inheritdoc/>
   public Task<Func<ClientMsg>> GetNotification()
   {
      lock (sync)
      {
         taskCompletion = new();

         if (isStopping)
         {
            isStopping = false;
            taskCompletion.SetCanceled();
         }
         else if (queue.TryDequeue(out var notification))
            taskCompletion.SetResult(notification);
      }

      return taskCompletion.Task;
   }

   /// <inheritdoc/>
   public void StopNotify()
   {
      lock (sync)
         taskCompletion?.TrySetCanceled();
      //lock (sync)
      //   if (!taskCompletion?.TrySetCanceled() ?? true)
      //      isStopping = true;
   }

   /// <inheritdoc/>
   public void NotificationQueueClear()
   {
      queue.Clear();
   }

   // https://stackoverflow.com/questions/54755889/do-tasks-generated-by-taskcompletionsource-need-to-be-disposed
   //public async Task<Packet> Wait()
   //{
   //    Packet packet;

   //    lock (sync)
   //        if (packets.TryDequeue(out packet))
   //            return packet;
   //        else
   //            waiter = new TaskCompletionSource<Packet>();

   //    return await waiter.Task;
   //}

   //public void Poke(Packet packet)
   //{
   //    lock (sync)
   //        if (waiter == null)
   //            packets.Enqueue(packet);
   //        else
   //            waiter.SetResult(packet);
   //}
}
