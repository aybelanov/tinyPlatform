namespace Hub.Core.Domain.Common;

 /// <summary>
 /// Represents a soft-deleted (without actually deleting from storage) entity
 /// </summary>
 public partial interface ISoftDeletedEntity
 {
     /// <summary>
     /// Gets or sets a value indicating whether the entity has been deleted
     /// </summary>
     bool IsDeleted { get; set; }
 }
