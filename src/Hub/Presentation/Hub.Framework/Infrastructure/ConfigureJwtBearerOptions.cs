using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.Core.Configuration;
using Hub.Core.Infrastructure;
using Hub.Services.Clients;
using Hub.Web.Framework.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Shared.Common;

namespace Hub.Web.Framework.Infrastructure;
#pragma warning disable CS1591

public class ConfigureJwtBearerOptions : IPostConfigureOptions<JwtBearerOptions>
{
   public void PostConfigure(string name, JwtBearerOptions options)
   {
      //var originalOnMessageReceived = options.Events.OnMessageReceived;
      options.Events.OnMessageReceived = async context =>
      {
         //await originalOnMessageReceived(context);

         if (string.IsNullOrEmpty(context.Token))
         {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(ClientDefaults.SignalrEndpoint))
            {
               context.Token = accessToken;
            }
         }

         await Task.CompletedTask;
      };
   }
}

#pragma warning restore CS1591