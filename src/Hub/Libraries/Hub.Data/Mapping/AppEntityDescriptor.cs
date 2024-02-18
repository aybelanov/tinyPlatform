using System.Collections.Generic;

namespace Hub.Data.Mapping;

/// <summary>
/// Represents an application entity de
/// </summary>
public class AppEntityDescriptor
{
   /// <summary>
   /// Default Ctor
   /// </summary>
   public AppEntityDescriptor()
   {
      Fields = new List<AppEntityFieldDescriptor>();
   }

   /// <summary>
   /// Entity name
   /// </summary>
   public string EntityName { get; set; }

   /// <summary>
   /// Field collection
   /// </summary>
   public ICollection<AppEntityFieldDescriptor> Fields { get; set; }
}