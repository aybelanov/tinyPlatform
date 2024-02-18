using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Web.Framework.TagHelpers.Shared;

/// <summary>
/// Uses for tri-state checkbox
/// </summary>
public enum BooleanNullable
{
   /// <summary>
   /// not set
   /// </summary>
   Any = 0,

   /// <summary>
   /// false
   /// </summary>
   False = 1,

   /// <summary>
   /// true
   /// </summary>
   True = 2,
}

/// <summary>
/// Extension for BooleanNullable enumerator
/// </summary>
public static class BooleanNullableExtension
{
   /// <summary>
   /// BooleanNullable to nullable boolean
   /// </summary>
   /// <param name="member">BooleanNullable emnum member</param>
   /// <returns>Boolean result</returns>
   public static bool? ToBoolean(this BooleanNullable member) => member switch
   {
      BooleanNullable.True => true,
      BooleanNullable.False => false,
      _ => null
   };
}
