﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hub.Core.Infrastructure
{
   /// <summary>
   /// Classes implementing this interface can serve as a portal for the various services composing the App engine. 
   /// Edit functionality, modules and implementations access most App functionality through this interface.
   /// </summary>
   public interface IEngine
   {
      /// <summary>
      /// Add and configure services
      /// </summary>
      /// <param name="services">Collection of service descriptors</param>
      /// <param name="configuration">Configuration of the application</param>
      void ConfigureServices(IServiceCollection services, IConfiguration configuration);

      /// <summary>
      /// Configure HTTP request pipeline
      /// </summary>
      /// <param name="application">Builder for configuring an application's request pipeline</param>
      void ConfigureRequestPipeline(IApplicationBuilder application);

      /// <summary>
      /// Resolve dependency
      /// </summary>
      /// <param name="scope">Scope</param>
      /// <typeparam name="T">Type of resolved service</typeparam>
      /// <returns>Resolved service</returns>
      T Resolve<T>(IServiceScope scope = null) where T : class;

      /// <summary>
      /// Resolve dependency
      /// </summary>
      /// <param name="type">Type of resolved service</param>
      /// <param name="scope">Scope</param>
      /// <returns>Resolved service</returns>
      object Resolve(Type type, IServiceScope scope = null);

      /// <summary>
      /// Resolve dependencies
      /// </summary>
      /// <typeparam name="T">Type of resolved services</typeparam>
      /// <returns>Collection of resolved services</returns>
      IEnumerable<T> ResolveAll<T>();

      /// <summary>
      /// Resolve unregistered service
      /// </summary>
      /// <param name="type">Type of service</param>
      /// <returns>Resolved service</returns>
      object ResolveUnregistered(Type type);

   }
}