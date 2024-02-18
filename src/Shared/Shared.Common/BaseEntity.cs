namespace Shared.Common;

/// <summary>
/// Represents the base class for entities
/// </summary>
public abstract partial class BaseEntity
{
   /// <summary>
   /// Gets or sets the entity identifier
   /// </summary>
   public long Id { get; set; }
}