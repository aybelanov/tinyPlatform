using Shared.Common;

namespace Hub.Core.Domain.Clients;

/// <summary>
/// Represents a command class
/// </summary>
public class Command : BaseEntity, ICommand
{
   /// <summary>
   /// Command name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Command arguments
   /// </summary>
   public string Arguments { get; set; }

   /// <summary>
   /// Device identifier
   /// </summary>
   public long DeviceId { get; set; }
}
