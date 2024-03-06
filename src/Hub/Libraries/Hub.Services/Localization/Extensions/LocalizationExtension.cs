using Hub.Core.Domain.Localization;
using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hub.Services.Localization.Extensions;

/// <summary>
/// Extension methods for localization
/// </summary>
public static class LocalizationExtension
{
   /// <summary>
   /// Gets property names of the entity marked as localizable
   /// </summary>
   /// <param name="entity"></param>
   /// <returns></returns>
   public static IEnumerable<string> GetLocalizablePropertyNames(this BaseEntity entity)
   {
      return GetLocalizablePropertyNames(entity.GetType());
   }

   /// <summary>
   /// Gets property names of the entity marked as localizable
   /// </summary>
   /// <param name="type">Entity type</param>
   /// <returns></returns>
   public static IEnumerable<string> GetLocalizablePropertyNames(this Type type)
   {
      var properties = type.GetProperties().Where(x => x.CanWrite && x.CanRead && x.PropertyType == typeof(string));

      foreach (var property in properties)
      {
         var attributes = property.GetCustomAttributes(true).ToList();

         foreach (var attribute in attributes)
         {
            if (attribute is LocalizableAttribute)
            {
               yield return property.Name;
            }
         }
      }
   }
}