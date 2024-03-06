namespace Shared.Devices;

/// <summary>
/// Represents a device interface
/// </summary>
public interface IDevice
{
   /// <summary>
   /// Device identifier
   /// </summary>
   long Id { get; set; }

   /// <summary>
   /// JSON configuration of the device
   /// </summary>
   string Configuration { get; set; }

   /// <summary>
   /// Max count of the records in a database for SensorRecords table
   /// </summary>
   int CountDataRows { get; set; }

   /// <summary>
   /// Delay between repeat of sending data stream in miliseconds  
   /// </summary>
   int DataSendingDelay { get; set; }

   /// <summary>
   /// Delay for repeat clearing the database in seconds
   /// </summary>
   int ClearDataDelay { get; set; }

   /// <summary>
   /// Size of data pakage for a seance of dataflow
   /// </summary>
   int DataPacketSize { get; set; }

   /// <summary>
   /// Delay between the data flow seances in miliseconds
   /// </summary>
   int DataflowReconnectDelay { get; set; }
}