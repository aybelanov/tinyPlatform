using System;

namespace Hub.Core.Domain.Common;

/// <summary>
/// Represents an interface for entities with controlled modification time
/// </summary>
public interface IModifiedEntity
{
   /// <summary>
   /// Created date of the entity
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

   /// <summary>
   /// Modified date of the entity
   /// </summary>
   public DateTime? UpdatedOnUtc { get; set; }
}
