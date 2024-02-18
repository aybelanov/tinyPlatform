using System;

namespace Hub.Data.Mapping;

#pragma warning disable CS1591

public class AppEntityFieldDescriptor
{
   public string Name { get; set; }
   public bool IsIdentity { get; set; }
   public bool? IsNullable { get; set; }
   public bool IsPrimaryKey { get; set; }
   public bool IsUnique { get; set; }
   public int? Precision { get; set; }
   public int? Size { get; set; }
   public Type Type { get; set; }
}

#pragma warning restore CS1591