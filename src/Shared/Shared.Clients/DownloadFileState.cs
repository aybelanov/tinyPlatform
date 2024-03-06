namespace Shared.Clients;

/// <summary>
/// Downloading file state
/// </summary>
public enum DownloadFileState
{
   /// <summary>
   /// File in the queue to handle
   /// </summary>
   InTheQueue = 1,

   /// <summary>
   /// Creating stet
   /// </summary>
   Processing,

   /// <summary>
   /// File is ready to download
   /// </summary>
   Ready,

   /// <summary>
   /// Dawnload task was canceled
   /// </summary>
   Canceled,

   /// <summary>
   /// Download file task is expired
   /// or data file was removed by the system
   /// </summary>
   Expired
}
