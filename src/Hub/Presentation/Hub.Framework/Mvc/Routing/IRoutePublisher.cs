﻿using Microsoft.AspNetCore.Routing;

namespace Hub.Web.Framework.Mvc.Routing
{
   /// <summary>
   /// Represents route publisher
   /// </summary>
   public interface IRoutePublisher
   {
      /// <summary>
      /// Register routes
      /// </summary>
      /// <param name="endpointRouteBuilder">Route builder</param>
      void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder);
   }
}
