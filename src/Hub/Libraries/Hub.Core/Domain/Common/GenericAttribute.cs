using Shared.Common;
using System;

namespace Hub.Core.Domain.Common;

/// <summary>
/// Represents a generic attribute
/// </summary>
public partial class GenericAttribute : BaseEntity
{
   /// <summary>
   /// Gets or sets the entity identifier
   /// </summary>
   public long EntityId { get; set; }

   /// <summary>
   /// Gets or sets the key group
   /// </summary>
   public string KeyGroup { get; set; }

   /// <summary>
   /// Gets or sets the key
   /// </summary>
   public string Key { get; set; }

   /// <summary>
   /// Gets or sets the value
   /// </summary>
   public string Value { get; set; }

   /// <summary>
   /// Gets or sets the created or updated date
   /// </summary>
   public DateTime? CreatedOrUpdatedDateUTC { get; set; }
}
