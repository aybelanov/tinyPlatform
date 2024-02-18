using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.Core.Configuration;
using Microsoft.OpenApi.Models;

namespace Hub.Web.Framework.Configuration
{
   /// <summary>
   /// Swagger configuration default settings
   /// </summary>
   public class SwaggerConfig : IConfig
   {
      /// <summary>
      /// Doc info
      /// </summary>
      public SwaggerDoc Doc { get; set; } = new();

      /// <summary>
      /// Enables/disables swagger tools
      /// </summary>
      public bool Enabled { get; internal set; } = true;
   }

   /// <summary>
   /// Swagger documentaion metadata
   /// </summary>
   public class SwaggerDoc
   {
      /// <inheritdoc cref="OpenApiInfo.Title"/>
      public string Title { get; internal set; }


      /// <inheritdoc cref="OpenApiInfo.Description"/>
      public string Description { get; internal set; }


      /// <inheritdoc cref="OpenApiInfo.TermsOfService"/>
      public Uri TermsOfService { get; internal set; } = new("http://tinyplat.com/termofservice");


      /// <inheritdoc cref="OpenApiInfo.Contact"/>
      public OpenApiContact Contact { get; internal set; } = new() { Email = "your@email.com", Name = "Contact name", Url = new("http://tinyplat.com") };


      /// <inheritdoc cref="OpenApiInfo.License"/>
      public OpenApiLicense License { get; internal set; } = new() { Url = new("http://tinyplat.com/license"), Name = "License name" };


      /// <inheritdoc cref="IConfig.GetOrder"/>
      public int GetOrder() => 100;
   }
}
