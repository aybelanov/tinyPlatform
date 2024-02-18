using Shared.Devices.Proto;
using System;
using System.Threading.Tasks;

namespace Devices.Dispatcher.Services;

/// <summary>
/// Represents a point-to-point service interface
/// </summary>
public interface IPoint2PointService
{
   /// <summary>
   /// Adds server notification as a function
   /// </summary>
   /// <param name="command">Function</param>
   void AddServerNotification(Func<ClientMsg> command);

   /// <summary>
   /// Adds server notification as a function
   /// </summary>
   /// <param name="msg">message</param>
   void AddServerNotification(ClientMsg msg);

   /// <summary>
   /// Gets a notification
   /// </summary>
   /// <returns>Client messsage</returns>
   Task<Func<ClientMsg>> GetNotification();

   /// <summary>
   /// Stops notification
   /// </summary>
   void StopNotify();

   /// <summary>
   /// Clears the notification queue
   /// </summary>
   void NotificationQueueClear();
}
