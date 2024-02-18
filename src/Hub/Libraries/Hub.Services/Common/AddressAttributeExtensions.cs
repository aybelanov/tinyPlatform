﻿using Hub.Core.Domain.Common;

namespace Hub.Services.Common
{
   /// <summary>
   /// Extensions
   /// </summary>
   public static class AddressAttributeExtensions
   {
      /// <summary>
      /// A value indicating whether this address attribute should have values
      /// </summary>
      /// <param name="addressAttribute">Address attribute</param>
      /// <returns>Result</returns>
      public static bool ShouldHaveValues(this AddressAttribute addressAttribute)
      {
         if (addressAttribute == null)
            return false;

         //other attribute control types support values
         return true;
      }
   }
}
