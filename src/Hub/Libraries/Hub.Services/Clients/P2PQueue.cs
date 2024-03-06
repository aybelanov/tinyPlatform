using Grpc.Core;
using Shared.Devices.Proto;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Hub.Services.Clients;

/// <summary>
/// Represents a queue fo point-to-point communication
/// </summary>
public class P2PQueue : ConcurrentQueue<Func<ServerMsg>>
{
   #region fields

   private TaskCompletionSource<Func<ServerMsg>> taskCompletion;
   private readonly object sync = new();
   private bool isStopping;
   private int _volume;
   private ServerCallContext _context;

   #endregion

   #region Ctors

   /// <summary>
   /// Default Ctor
   /// </summary>
   /// <param name="volume">Queue max volume</param>
   /// <param name="context">Server call context</param>
   public P2PQueue(ServerCallContext context, int volume) : base()
   {
      _context = context;
      _volume = volume;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Adds message to the queue
   /// </summary>
   /// <param name="command">Server message delegate</param>
   public void AddMessage(Func<ServerMsg> command)
   {
      lock (sync)
      {
         if (taskCompletion == null || !taskCompletion.TrySetResult(command))
         {
            if (_volume > 0 && Count + 1 > _volume)
               _ = TryDequeue(out _);

            Enqueue(command);
         }
      }
   }

   /// <summary>
   /// Get message to send to the device
   /// </summary>
   /// <returns>Server message delegate</returns>
   public Task<Func<ServerMsg>> GetMessage()
   {
      lock (sync)
      {
         taskCompletion = new();

         if (isStopping)
         {
            isStopping = false;
            taskCompletion.SetCanceled();
         }
         else if (TryDequeue(out var message))
         {
            taskCompletion.SetResult(message);
         }
      }

      return taskCompletion.Task;
   }

   /// <summary>
   /// Stops message queueing and getting message
   /// </summary>
   public void Stop()
   {
      lock (sync)
      {
         if (taskCompletion == null || !taskCompletion.TrySetCanceled())
            isStopping = true;
      }
   }

   #endregion

   #region Properties

   /// <summary>
   /// Server call context
   /// </summary>
   public ServerCallContext CallContext => _context;

   #endregion
}
