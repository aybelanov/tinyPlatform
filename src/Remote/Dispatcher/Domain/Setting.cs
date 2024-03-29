﻿using Shared.Common;

namespace Devices.Dispatcher.Domain;

/// <summary>
/// Represents a setting
/// </summary>
public partial class Setting : BaseEntity
{
   /// <summary>
   /// Default Ctor
   /// </summary>
   public Setting()
   {
   }

   /// <summary>
   /// Ctor
   /// </summary>
   /// <param name="name">Name</param>
   /// <param name="value">Value</param>
   public Setting(string name, string value)
   {
      Name = name;
      Value = value;
   }

   /// <summary>
   /// Gets or sets the name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Gets or sets the value
   /// </summary>
   public string Value { get; set; }

   /// <summary>
   /// To string
   /// </summary>
   /// <returns>Result</returns>
   public override string ToString()
   {
      return Name;
   }
}