﻿using Hub.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Linq;

namespace Hub.Web.Framework.Mvc.ModelBinding
{
   /// <summary>
   /// Represents metadata provider that adds custom attributes to the model's metadata, so it can be retrieved later
   /// </summary>
   public class AppMetadataProvider : IDisplayMetadataProvider
   {
      /// <summary>
      /// Sets the values for properties of isplay metadata
      /// </summary>
      /// <param name="context">Display metadata provider context</param>
      public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
      {
         //get all custom attributes
         var additionalValues = context.Attributes.OfType<IModelAttribute>().ToList();

         //and try add them as additional values of metadata
         foreach (var additionalValue in additionalValues)
         {
            if (context.DisplayMetadata.AdditionalValues.ContainsKey(additionalValue.Name))
               throw new AppException("There is already an attribute with the name '{0}' on this model", additionalValue.Name);

            context.DisplayMetadata.AdditionalValues.Add(additionalValue.Name, additionalValue);
         }
      }
   }
}