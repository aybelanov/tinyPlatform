using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clients.Dash.Shared.Communication;

/// <summary>
/// Represents UI control for data load process
/// </summary>
public class DataLoadProcess
{
   #region fields

   private readonly Queue<object> _queue = new();

   #endregion

   #region Events

   /// <summary>
   /// Data load process state has changed
   /// </summary>
   public event Action LoadProcessStarting;

   /// <summary>
   /// Data load process state has changed
   /// </summary>
   public event Func<Task> LoadProcessEnded;

   #endregion

   #region Methods

   /// <summary>
   /// On UI control for data load process
   /// </summary>
   public void On()
   {
      _queue.Enqueue(new object());
      LoadProcessStarting?.Invoke();
   }

   /// <summary>
   /// Off UI control for data load process
   /// </summary>
   public void Off()
   {
      _ = _queue.TryDequeue(out _);

      if (_queue.Count == 0)
         LoadProcessEnded?.Invoke();
   }

   /// <summary>
   /// Clear load queue and stop UI control data load process 
   /// </summary>
   public void ClearLoading()
   {
      _queue.Clear();
      //InProgress = false;
      LoadProcessEnded?.Invoke();
   }

   #endregion
}