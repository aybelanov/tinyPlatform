namespace Shared.Common;

/// <summary>
/// Represents a device command interface
/// </summary>
public interface ICommand
{
   /// <summary>
   /// Command arguments
   /// </summary>
   string Arguments { get; set; }

   /// <summary>
   /// Device identifier
   /// </summary>
   long DeviceId { get; set; }

   /// <summary>
   /// Command name
   /// </summary>
   string Name { get; set; }
}