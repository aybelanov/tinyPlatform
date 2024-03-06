using Hub.Core;
using Hub.Core.Configuration;
using Hub.Core.Domain.Common;
using Hub.Core.Http;
using Hub.Core.Infrastructure;
using Hub.Data;
using Hub.Services.Authentication;
using Hub.Services.Clients;
using Hub.Services.Common;
using Hub.Services.Installation;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Media;
using Hub.Services.Media.RoxyFileman;
using Hub.Services.Plugins;
using Hub.Services.ScheduleTasks;
using Hub.Web.Framework.Configuration;
using Hub.Web.Framework.Globalization;
using Hub.Web.Framework.Mvc.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using WebMarkupMin.AspNetCore7;
using WebOptimizer;

namespace Hub.Web.Framework.Infrastructure.Extensions;

/// <summary>
/// Represents extensions of IApplicationBuilder
/// </summary>
public static class ApplicationBuilderExtensions
{
   /// <summary>
   /// Configure the application HTTP request pipeline
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void ConfigureRequestPipeline(this IApplicationBuilder application)
   {
      EngineContext.Current.ConfigureRequestPipeline(application);
   }

   /// <summary>
   /// Starts app Engine
   /// </summary>
   /// <param name="application"></param>
   public static async Task StartEngineAsync(this IApplicationBuilder application)
   {
      //further actions are performed only when the database is installed
      if (DataSettingsManager.IsDatabaseInstalled())
      {
         var engine = EngineContext.Current;

         //log application start
         await engine.Resolve<ILogger>().InformationAsync("Application started");

         //install and update plugins
         var pluginService = engine.Resolve<IPluginService>();
         await pluginService.InstallPluginsAsync();
         await pluginService.UpdatePluginsAsync();

         //update application db
         using (var serviceScope = application.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
         {
            var dbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
            await dbContext.Database.MigrateAsync();
         }

         var taskScheduler = engine.Resolve<ITaskScheduler>();
         await taskScheduler.InitializeAsync();
         taskScheduler.StartScheduler();
      }
   }

   /// <summary>
   /// Add exception handling
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void UseAppExceptionHandler(this IApplicationBuilder application)
   {
      var appSettings = EngineContext.Current.Resolve<AppSettings>();
      var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();
      var useDetailedExceptionPage = appSettings.Get<CommonConfig>().DisplayFullErrorStack || webHostEnvironment.IsDevelopment();
      if (useDetailedExceptionPage)
         //get detailed exceptions for developing and testing purposes
         application.UseDeveloperExceptionPage();
      else
         //or use special exception handler
         application.UseExceptionHandler("/Error/Error");

      //log errors
      application.UseExceptionHandler(handler =>
      {
         handler.Run(async context =>
         {
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (exception == null)
               return;

            try
            {
               //check whether database is installed
               if (DataSettingsManager.IsDatabaseInstalled())
               {
                  //get current user
                  var currentUser = await EngineContext.Current.Resolve<IWorkContext>().GetCurrentUserAsync();

                  //log error
                  await EngineContext.Current.Resolve<ILogger>().ErrorAsync(exception.Message, exception, currentUser);
               }
            }
            finally
            {
               //rethrow the exception to show the error page
               ExceptionDispatchInfo.Throw(exception);
            }
         });
      });
   }

   /// <summary>
   /// Adds a special handler that checks for responses with the 404 status code that do not have a body
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void UsePageNotFound(this IApplicationBuilder application)
   {
      application.UseStatusCodePages(async context =>
      {
         //handle 404 Not Found
         if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
         {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            if (!webHelper.IsStaticResource())
            {
               //get original path and query
               var originalPath = context.HttpContext.Request.Path;
               var originalQueryString = context.HttpContext.Request.QueryString;

               if (DataSettingsManager.IsDatabaseInstalled())
               {
                  var commonSettings = EngineContext.Current.Resolve<CommonSettings>();

                  if (commonSettings.Log404Errors)
                  {
                     var logger = EngineContext.Current.Resolve<ILogger>();
                     var workContext = EngineContext.Current.Resolve<IWorkContext>();

                     await logger.ErrorAsync($"Error 404. The requested page ({originalPath}) was not found",
                            user: await workContext.GetCurrentUserAsync());
                  }
               }

               try
               {
                  //get new path
                  var pageNotFoundPath = "/page-not-found";
                  //re-execute request with new path
                  context.HttpContext.Response.Redirect(context.HttpContext.Request.PathBase + pageNotFoundPath);
               }
               finally
               {
                  //return original path to request
                  context.HttpContext.Request.QueryString = originalQueryString;
                  context.HttpContext.Request.Path = originalPath;
               }
            }
         }
      });
   }

   /// <summary>
   /// Adds a special handler that checks for responses with the 400 status code (bad request)
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void UseBadRequestResult(this IApplicationBuilder application)
   {
      application.UseStatusCodePages(async context =>
      {
         //handle 404 (Bad request)
         if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
         {
            var logger = EngineContext.Current.Resolve<ILogger>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            await logger.ErrorAsync("Error 400. Bad request", null, user: await workContext.GetCurrentUserAsync());
         }
      });
   }

   /// <summary>
   /// Configure middleware for dynamically compressing HTTP responses
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void UseAppResponseCompression(this IApplicationBuilder application)
   {
      if (!DataSettingsManager.IsDatabaseInstalled())
         return;

      //whether to use compression (gzip by default)
      if (EngineContext.Current.Resolve<CommonSettings>().UseResponseCompression)
         application.UseResponseCompression();
   }

   /// <summary>
   /// Adds WebOptimizer to the <see cref="IApplicationBuilder"/> request execution pipeline
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void UseAppWebOptimizer(this IApplicationBuilder application)
   {
      var fileProvider = EngineContext.Current.Resolve<IAppFileProvider>();
      var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();

      application.UseWebOptimizer(webHostEnvironment, new[]
      {
             new FileProviderOptions
             {
                 RequestPath =  new PathString("/Plugins"),
                 FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Plugins"))
             },
             new FileProviderOptions
             {
                 RequestPath =  new PathString("/Themes"),
                 FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Themes"))
             }
         });
   }

   /// <summary>
   /// Configure static file serving
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void UseStaticIpCamFiles(this IApplicationBuilder application)
   {
      application.UseMiddleware<VideoCacheMiddleware>();
      application.UseStaticFiles(new StaticFileOptions
      {
         RequestPath = new PathString(ClientDefaults.IpCamEndpoint),
         FileProvider = new PhysicalFileProvider(CommonHelper.DefaultFileProvider.GetAbsolutePath(ClientDefaults.VideoStorageDirectory)),
         ContentTypeProvider = new FileExtensionContentTypeProvider()
         {
            Mappings =
            {
               [".ts"] = "video/mp2t",
               [".m4s"] = "video/iso.segment",
               [".webm"] = "video/webm"
            }
         },
         OnPrepareResponse = ctx =>
         {
            var cacheControl = Singleton<AppSettings>.Instance.Get<CommonConfig>().StaticIpCamFilesCacheControl;
            if (!string.IsNullOrEmpty(cacheControl))
               ctx.Context.Response.Headers.Append(HeaderNames.CacheControl, cacheControl);
         },
      });
   }

   /// <summary>
   /// Configure static file serving
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void UseAppStaticFiles(this IApplicationBuilder application)
   {
      var fileProvider = EngineContext.Current.Resolve<IAppFileProvider>();
      var appSettings = EngineContext.Current.Resolve<AppSettings>();

      void staticFileResponse(StaticFileResponseContext context)
      {
         if (!string.IsNullOrEmpty(appSettings.Get<CommonConfig>().StaticFilesCacheControl))
            context.Context.Response.Headers.Append(HeaderNames.CacheControl, appSettings.Get<CommonConfig>().StaticFilesCacheControl);
      }

      //common static files
      application.UseStaticFiles(new StaticFileOptions { OnPrepareResponse = staticFileResponse });

      //themes static files
      application.UseStaticFiles(new StaticFileOptions
      {
         FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Themes")),
         RequestPath = new PathString("/Themes"),
         OnPrepareResponse = staticFileResponse
      });

      //plugins static files
      var staticFileOptions = new StaticFileOptions
      {
         FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Plugins")),
         RequestPath = new PathString("/Plugins"),
         OnPrepareResponse = staticFileResponse
      };

      //exclude files in blacklist
      if (!string.IsNullOrEmpty(appSettings.Get<CommonConfig>().PluginStaticFileExtensionsBlacklist))
      {
         var fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();

         foreach (var ext in appSettings.Get<CommonConfig>().PluginStaticFileExtensionsBlacklist
             .Split(';', ',')
             .Select(e => e.Trim().ToLowerInvariant())
             .Select(e => $"{(e.StartsWith(".") ? string.Empty : ".")}{e}")
             .Where(fileExtensionContentTypeProvider.Mappings.ContainsKey))
            fileExtensionContentTypeProvider.Mappings.Remove(ext);

         staticFileOptions.ContentTypeProvider = fileExtensionContentTypeProvider;
      }

      application.UseStaticFiles(staticFileOptions);

      //add support for backups
      var provider = new FileExtensionContentTypeProvider
      {
         Mappings = { [".bak"] = MimeTypes.ApplicationOctetStream }
      };

      application.UseStaticFiles(new StaticFileOptions
      {
         FileProvider = new PhysicalFileProvider(fileProvider.MapPath(HubCommonDefaults.DbBackupsPath)),
         RequestPath = new PathString("/db_backups"),
         ContentTypeProvider = provider
      });

      // report data files
      application.UseStaticFiles(new StaticFileOptions
      {
         FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath(ClientDefaults.ReportFilleDirectory)),
         RequestPath = new PathString("/reportfiles"),
         ContentTypeProvider = new FileExtensionContentTypeProvider()
         {
            Mappings =
            {
               [".json"] = MimeTypes.ApplicationOctetStream,
               [".xml"] = MimeTypes.ApplicationOctetStream,
               [".txt"] = MimeTypes.ApplicationOctetStream,
               [".csv"] = MimeTypes.ApplicationOctetStream,
               [".zip"] = MimeTypes.ApplicationOctetStream,
               [".gzip"] = MimeTypes.ApplicationOctetStream
            }
         },
         OnPrepareResponse = context =>
         {
            staticFileResponse(context);
            context.Context.Response.Headers.Append(HeaderNames.ContentDisposition, "attachment");
         }
      });

      //add support for webmanifest files
      provider.Mappings[".webmanifest"] = MimeTypes.ApplicationManifestJson;

      application.UseStaticFiles(new StaticFileOptions
      {
         FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath("icons")),
         RequestPath = "/icons",
         ContentTypeProvider = provider
      });

      if (DataSettingsManager.IsDatabaseInstalled())
         application.UseStaticFiles(new StaticFileOptions
         {
            FileProvider = EngineContext.Current.Resolve<IRoxyFilemanFileProvider>(),
            RequestPath = new PathString(AppRoxyFilemanDefaults.DefaultRootDirectory),
            OnPrepareResponse = staticFileResponse
         });

      if (appSettings.Get<CommonConfig>().ServeUnknownFileTypes)
         application.UseStaticFiles(new StaticFileOptions
         {
            FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath(".well-known")),
            RequestPath = new PathString("/.well-known"),
            ServeUnknownFileTypes = true,
         });
   }

   /// <summary>
   /// Configure CORS middleware 
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void UseAppCors(this IApplicationBuilder application)
   {
      var securityConfig = EngineContext.Current.Resolve<AppSettings>().Get<SecurityConfig>();
      if (securityConfig.CorsEnabled)
      {
         // Add additional CORS header for Blazor wasm standalone:
         application.Use(async (context, next) =>
         {
            var originsString = string.Join(' ', securityConfig.CorsOrigins);
            context.Response.Headers.Append("X-Frame-Options", new($"ALLOW-FROM {originsString}"));
            context.Response.Headers.Append("Content-Security-Policy", new($"frame-ancestors 'self' {originsString}"));
            //context.Response.Headers.Add("Access-Control-Allow-Origin", originsString);
            await next.Invoke();
         });
      }

      application.UseCors(WebFrameworkDefaults.CorsPolicyName);
   }

   /// <summary>
   /// Configure middleware checking whether requested page is keep alive page
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void UseKeepAlive(this IApplicationBuilder application)
   {
      application.UseMiddleware<KeepAliveMiddleware>();
   }

   /// <summary>
   /// Configure middleware checking whether database is installed
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void UseInstallUrl(this IApplicationBuilder application)
   {
      application.UseMiddleware<InstallUrlMiddleware>();
   }

   /// <summary>
   /// Adds the authentication middleware, which enables authentication capabilities.
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void UseAppAuthentication(this IApplicationBuilder application)
   {
      //check whether database is installed
      if (!DataSettingsManager.IsDatabaseInstalled())
         return;

      //application.UseAuthentication();
      application.UseMiddleware<AuthenticationMiddleware>();
   }


   /// <summary>
   /// Configure the request localization feature
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void UseAppRequestLocalization(this IApplicationBuilder application)
   {
      application.UseRequestLocalization(async options =>
      {
         if (!DataSettingsManager.IsDatabaseInstalled())
            return;

         //prepare supported cultures
         var cultures = (await EngineContext.Current.Resolve<ILanguageService>().GetAllLanguagesAsync())
                .OrderBy(language => language.DisplayOrder)
                .Select(language => new CultureInfo(language.LanguageCulture)).ToList();
         options.SupportedCultures = cultures;
         options.SupportedUICultures = cultures;
         options.DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault());
         options.ApplyCurrentCultureToResponseHeaders = true;

         //configure culture providers
         options.AddInitialRequestCultureProvider(new AppSeoUrlCultureProvider());
         var cookieRequestCultureProvider = options.RequestCultureProviders.OfType<CookieRequestCultureProvider>().FirstOrDefault();
         if (cookieRequestCultureProvider is not null)
            cookieRequestCultureProvider.CookieName = $"{AppCookieDefaults.Prefix}{AppCookieDefaults.CultureCookie}";
      });
   }

   /// <summary>
   /// Configure Endpoints routing
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void UseMvcEndpoints(this IApplicationBuilder application)
   {
      //Execute the endpoint selected by the routing middleware
      application.UseEndpoints(endpoints =>
      {
         //register all routes
         EngineContext.Current.Resolve<IRoutePublisher>().RegisterRoutes(endpoints);
      });
   }

   /// <summary>
   /// Configure applying forwarded headers to their matching fields on the current request.
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void UseAppProxy(this IApplicationBuilder application)
   {
      var appSettings = EngineContext.Current.Resolve<AppSettings>();

      if (appSettings.Get<HostingConfig>().UseProxy)
      {
         var options = new ForwardedHeadersOptions
         {
            ForwardedHeaders = ForwardedHeaders.All,
            // IIS already serves as a reverse proxy and will add X-Forwarded headers to all requests,
            // so we need to increase this limit, otherwise, passed forwarding headers will be ignored.
            ForwardLimit = 2
         };

         if (!string.IsNullOrEmpty(appSettings.Get<HostingConfig>().ForwardedForHeaderName))
            options.ForwardedForHeaderName = appSettings.Get<HostingConfig>().ForwardedForHeaderName;

         if (!string.IsNullOrEmpty(appSettings.Get<HostingConfig>().ForwardedProtoHeaderName))
            options.ForwardedProtoHeaderName = appSettings.Get<HostingConfig>().ForwardedProtoHeaderName;

         if (!string.IsNullOrEmpty(appSettings.Get<HostingConfig>().KnownProxies))
         {
            foreach (var strIp in appSettings.Get<HostingConfig>().KnownProxies.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
               if (IPAddress.TryParse(strIp, out var ip))
                  options.KnownProxies.Add(ip);

            if (options.KnownProxies.Count > 1)
               options.ForwardLimit = null; //disable the limit, because KnownProxies is configured
         }

         //configure forwarding
         application.UseForwardedHeaders(options);
      }
   }

   /// <summary>
   /// Configure WebMarkupMin
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void UseAppWebMarkupMin(this IApplicationBuilder application)
   {
      //check whether database is installed
      if (!DataSettingsManager.IsDatabaseInstalled())
         return;

      application.UseWebMarkupMin();
   }

   /// <summary>
   /// Configure Swagger middleware
   /// </summary>
   /// <param name="application">Builder for configuring an application's request pipeline</param>
   public static void UseAppSwagger(this IApplicationBuilder application)
   {
      //check whether database is installed
      if (!DataSettingsManager.IsDatabaseInstalled())
         return;

      application.UseSwagger();
      application.UseSwaggerUI();
   }

   /// <summary>
   /// Configures Rate limiter
   /// </summary>
   /// <param name="application"></param>
   public static void UseAppRateLimiter(this IApplicationBuilder application)
   {
      //check whether database is installed
      if (DataSettingsManager.IsDatabaseInstalled()
      && EngineContext.Current.Resolve<AppSettings>().Get<RateLimitConfig>().Enabled)
      {
         application.UseRateLimiter();
      }
   }

   /// <summary>
   /// Adds demo mode middleware
   /// </summary>
   /// <param name="application"></param>
   public static void UseDemoMode(this IApplicationBuilder application)
   {
      if (DataSettingsManager.IsDatabaseInstalled())
      {
         application.UseMiddleware<DemoMiddleware>();
      }
   }
}
