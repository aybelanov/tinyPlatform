using FluentValidation;
using FluentValidation.AspNetCore;
using Hub.Core;
using Hub.Core.Configuration;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Users;
using Hub.Core.Http;
using Hub.Core.Infrastructure;
using Hub.Core.Security;
using Hub.Data;
using Hub.Services.Authentication;
using Hub.Services.Authentication.External;
using Hub.Services.Clients;
using Hub.Services.Common;
using Hub.Services.Devices;
using Hub.Services.Security;
using Hub.Services.Users;
using Hub.Web.Framework.Configuration;
using Hub.Web.Framework.Mvc.ModelBinding;
using Hub.Web.Framework.Mvc.ModelBinding.Binders;
using Hub.Web.Framework.Mvc.Routing;
using Hub.Web.Framework.Security.Captcha;
using Hub.Web.Framework.Themes;
using Hub.Web.Framework.Validators;
using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shared.Clients.Configuration;
using StackExchange.Profiling.Storage;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using WebMarkupMin.AspNetCore7;
using WebMarkupMin.NUglify;

namespace Hub.Web.Framework.Infrastructure.Extensions;

/// <summary>
/// Represents extensions of IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
   /// <summary>
   /// Add services to the application and configure service provider
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   /// <param name="builder">A builder for web applications and services</param>
   public static void ConfigureApplicationServices(this IServiceCollection services,
       WebApplicationBuilder builder)
   {
      //let the operating system decide what TLS protocol version to use
      //see https://docs.microsoft.com/dotnet/framework/network-programming/tls
      ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;

      //create default file provider
      CommonHelper.DefaultFileProvider = new AppFileProvider(builder.Environment);

      //add accessor to HttpContext
      services.AddHttpContextAccessor();

      //initialize plugins
      var mvcCoreBuilder = services.AddMvcCore();
      var pluginConfig = new PluginConfig();
      builder.Configuration.GetSection(nameof(PluginConfig)).Bind(pluginConfig, options => options.BindNonPublicProperties = true);
      mvcCoreBuilder.PartManager.InitializePlugins(pluginConfig);

      //register type finder
      var typeFinder = new WebAppTypeFinder();
      Singleton<ITypeFinder>.Instance = typeFinder;
      services.AddSingleton<ITypeFinder>(typeFinder);

      //add configuration parameters
      // get default configuration
      var configurations = typeFinder
          .FindClassesOfType<IConfig>()
          .Select(configType => (IConfig)Activator.CreateInstance(configType))
          .ToList();

      // override according to the existing configuration
      var path = CommonHelper.DefaultFileProvider.MapPath(AppConfigurationDefaults.AppSettingsFilePath);
      Dictionary<string, object> currentSettings = null;
      if (CommonHelper.DefaultFileProvider.FileExists(path))
      {
         var str = File.ReadAllText(path);
         currentSettings = JsonConvert.DeserializeObject<Dictionary<string, object>>(str);
      }

      foreach (var config in configurations)
      {
         if (config.HasOwnSection())
            builder.Configuration.GetSection(config.Name).Bind(config, options => options.BindNonPublicProperties = true);
         else if (currentSettings is not null)
            config.GetType().GetProperties().ToList().ForEach(x =>
            {
               if (currentSettings.TryGetValue(x.Name, out var value))
                  x.SetValue(config, value);
            });
      }

      var appSettings = AppSettingsHelper.SaveAppSettings(configurations, CommonHelper.DefaultFileProvider, true);
      services.AddSingleton(appSettings);

      //create engine and configure service provider
      var engine = EngineContext.Create();

      engine.ConfigureServices(services, builder.Configuration);
   }


   /// <summary>
   /// Adds EF Core DBContext
   /// </summary>
   /// <typeparam name="TDbContext">DB context implementation class</typeparam>
   /// <param name="services">Service collection</param>
   /// 
   public static void AddAppDbContext<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
   {
      //if (DataSettingsManager.LoadSettings().DataProvider == DataProviderType.PostgreSQL)
      //   AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

      LinqToDBForEFTools.Initialize();

      services.AddDbContext<TDbContext>(options =>
      {
         var dataConfig = DataSettingsManager.LoadSettings();

         (dataConfig.DataProvider switch
         {
            DataProviderType.SqlServer => options.UseSqlServer(dataConfig.ConnectionString, x => x.MigrationsAssembly("Hub.Data.SqlServerMigrations")),
            DataProviderType.PostgreSQL => options.UseNpgsql(dataConfig.ConnectionString, x => x.MigrationsAssembly("Hub.Data.PostgreSqlMigrations")),
            DataProviderType.SQLite => options.UseSqlite(dataConfig.ConnectionString, x => x.MigrationsAssembly("Hub.Data.SqliteMigrations")),
            DataProviderType.MySql => options.UseMySql(ServerVersion.AutoDetect(dataConfig.ConnectionString), x => x.MigrationsAssembly("Hub.Data.MySqlMigrations")),
            _ => throw new AppException($"The data provider {dataConfig.DataProvider} is not supported.")
         })
         .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
         .UseLinqToDB();

#if DEBUG
         //options.LogTo(Console.WriteLine).EnableDetailedErrors();
         //options.EnableSensitiveDataLogging();
#endif

      });
   }


   /// <summary>
   /// Register HttpContextAccessor
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   public static void AddHttpContextAccessor(this IServiceCollection services)
   {
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
   }

   /// <summary>
   /// Adds services for rouiting
   /// </summary>
   /// <param name="services"></param>
   public static void AddAppRouting(this IServiceCollection services)
   {
      services.AddRouting(options => options.ConstraintMap[AppPathRouteDefaults.LanguageParameterTransformer] = typeof(LanguageParameterTransformer));
   }

   /// <summary>
   /// Adds services required for anti-forgery support
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   public static void AddAntiForgery(this IServiceCollection services)
   {
      //override cookie name
      services.AddAntiforgery(options =>
      {
         options.Cookie.Name = $"{AppCookieDefaults.Prefix}{AppCookieDefaults.AntiforgeryCookie}";
         options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
      });
   }


   /// <summary>
   /// Adds services required for application session state
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   public static void AddHttpSession(this IServiceCollection services)
   {
      services.AddSession(options =>
      {
         options.IdleTimeout = TimeSpan.FromSeconds(10);
         options.Cookie.Name = $"{AppCookieDefaults.Prefix}{AppCookieDefaults.SessionCookie}";
         options.Cookie.HttpOnly = true;
         options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
      });
   }


   /// <summary>
   /// Adds services required for themes support
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   public static void AddThemes(this IServiceCollection services)
   {
      if (!DataSettingsManager.IsDatabaseInstalled())
         return;

      //themes support
      services.Configure<RazorViewEngineOptions>(options
         => options.ViewLocationExpanders.Add(new ThemeableViewLocationExpander()));
   }


   /// <summary>
   /// Adds services required for distributed cache
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   public static void AddDistributedCache(this IServiceCollection services)
   {
      var appSettings = Singleton<AppSettings>.Instance;
      var distributedCacheConfig = appSettings.Get<DistributedCacheConfig>();

      if (!distributedCacheConfig.Enabled)
         return;

      switch (distributedCacheConfig.DistributedCacheType)
      {
         case DistributedCacheType.Memory:
            services.AddDistributedMemoryCache();
            break;

         case DistributedCacheType.SqlServer:
            services.AddDistributedSqlServerCache(options =>
            {
               options.ConnectionString = distributedCacheConfig.ConnectionString;
               options.SchemaName = distributedCacheConfig.SchemaName;
               options.TableName = distributedCacheConfig.TableName;
            });
            break;

         case DistributedCacheType.Redis:
            services.AddStackExchangeRedisCache(options =>
            {
               options.Configuration = distributedCacheConfig.ConnectionString;
            });
            break;
      }
   }

   /// <summary>
   /// Adds data protection services
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   public static void AddAppDataProtection(this IServiceCollection services)
   {
      var dataProtectionKeysPath = CommonHelper.DefaultFileProvider.MapPath(AppDataProtectionDefaults.DataProtectionKeysPath);
      var dataProtectionKeysFolder = new DirectoryInfo(dataProtectionKeysPath);

      //configure the data protection system to persist keys to the specified directory
      services.AddDataProtection()
         .PersistKeysToFileSystem(dataProtectionKeysFolder)
         .SetApplicationName(WebFrameworkDefaults.SolutionName);

      // init rsa security key (create if not exist or rebuild from a file)
      EncryptionHelper.GenerateOrRebuildRsaSecurityKey();

      // init symmetric security key (create if not exist or rebuild from a key in the file)
      EncryptionHelper.GenerateOrRebuildSymetricSecurityKey();
   }

   /// <summary>
   /// Adds an application authentication service
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   public static void AddAppAuthentication(this IServiceCollection services)
   {
      //set default authentication schemes
      var authenticationBuilder = services.AddAuthentication(options =>
      {
         options.DefaultChallengeScheme = AuthDefaults.MixedScheme;
         options.DefaultScheme = AuthDefaults.MixedScheme;
         options.DefaultSignInScheme = AuthDefaults.ExternalAuthenticationScheme;
      });

      // policy to apply a proper scheme
      authenticationBuilder.AddPolicyScheme(AuthDefaults.MixedScheme, AuthDefaults.MixedScheme, options =>
      { 
         options.ForwardDefaultSelector = context =>
         {
            // filter by auth type
            string authorization = context.Request.Headers[HeaderNames.Authorization];

            if ((!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
            // or it's a SignalR request
            || context.Request.Path.StartsWithSegments(ClientDefaults.SignalrEndpoint))
            {
               return JwtBearerDefaults.AuthenticationScheme;
            }

            // otherwise always check for cookie auth
            return AuthDefaults.CookieAuthenticationScheme;
         };
      });

      //add main cookie authentication
      authenticationBuilder.AddCookie(AuthDefaults.CookieAuthenticationScheme, options =>
      {
         options.Cookie.Name = $"{AppCookieDefaults.Prefix}{AppCookieDefaults.AuthenticationCookie}";
         options.Cookie.HttpOnly = true;
         options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
         options.LoginPath = AuthDefaults.LoginPath;
         options.LogoutPath = new("/logout");
         options.AccessDeniedPath = AuthDefaults.AccessDeniedPath;
         options.Events.OnValidatePrincipal = async context =>
         {
            // TODO check user for relogin 
            // context.RejectPrincipal();
            await Task.CompletedTask;
         };
      });

      //add external authentication
      authenticationBuilder.AddCookie(AuthDefaults.ExternalAuthenticationScheme, options =>
      {
         options.Cookie.Name = $"{AppCookieDefaults.Prefix}{AppCookieDefaults.ExternalAuthenticationCookie}";
         options.Cookie.HttpOnly = true;
         options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
         options.LoginPath = AuthDefaults.LoginPath;
         options.AccessDeniedPath = AuthDefaults.AccessDeniedPath;
      });

#if DEBUG
      IdentityModelEventSource.ShowPII = true;
#endif

      // add jwt authentication
      authenticationBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
      {
         var communicator = EngineContext.Current.Resolve<ICommunicator>();
         var appSettings = EngineContext.Current.Resolve<AppSettings>();
         var commonConfig = appSettings.Get<CommonConfig>();
         var hostingConfig = appSettings.Get<HostingConfig>();

         options.Authority = hostingConfig.HubHostUrl.TrimEnd('/');
         options.Events = new JwtBearerEvents
         {
            OnTokenValidated = async context =>
            {
               // TODO refactor to "relogin required or acces denied"
               string reason = null;
               var now = DateTime.UtcNow;
               var client = context.Principal.FindFirstValue("client_id");
               using var scope = context.HttpContext.RequestServices.CreateScope();

               if (client.Equals(AuthDefaults.DispatcherApp))
               {
                  var systemName = context.Principal.FindFirstValue("name");
                  if (!string.IsNullOrEmpty(systemName))
                  {
                     var deviceService = scope.ServiceProvider.GetRequiredService<IHubDeviceService>();
                     var device = await deviceService.GetDeviceBySystemNameAsync(systemName); 

                     if (device == null) reason = "Device does not exist.";
                     //else if (device.IsDeleted) reason = "Device was deleted.";
                     else if (!device.IsActive) reason = "Device was blocked.";
                     else if (device.CannotLoginUntilDateUtc.HasValue && device.CannotLoginUntilDateUtc > now) reason = $"Device cannot login until {now:G}";
                  }
                  else reason = "Device sytemname is empty";
               }
               else if (client.Equals(AuthDefaults.ClientApp))
               {
                  var userName = context.Principal.FindFirstValue("name") ?? context.Principal.FindFirstValue(ClaimTypes.Name);
                  if (!string.IsNullOrEmpty(userName))
                  {
                     var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                     var userSettings = scope.ServiceProvider.GetRequiredService<UserSettings>();
                     var user = userSettings.UsernamesEnabled ? await userService.GetUserByUsernameAsync(userName) : await userService.GetUserByEmailAsync(userName);

                     if (user == null) reason = "User does not exist.";
                     //else if (user.IsDeleted) reason = "User was deleted.";
                     else if (!user.IsActive) reason = "User was blocked.";
                     else if (user.CannotLoginUntilDateUtc.HasValue && user.CannotLoginUntilDateUtc > now) reason = $"User cannot login until {now:G}";
                     else if (context.HttpContext.Request.Path.StartsWithSegments(ClientDefaults.SignalrEndpoint)) // signalr connection count control
                     {
                        var userConnections = await communicator.GetUserConnectionsInfoAsync(user.Id);
                        if (!await userService.IsAdminAsync(user) && userConnections.Count + 1 > commonConfig.ConnectionPerUser)
                        {
                           var ips = string.Join(", ", userConnections.Select(x => x.IpAddressV4.ToString()));
                           reason = $"User {user.Email} has already connected from IP(s): {ips}.";
                        }
                     }
                  }
                  else
                  {
                     reason = "Username is empty.";
                  }
               }
               else reason = "Unknown client.";

               if (!string.IsNullOrEmpty(reason))
               {
                  context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                  await context.Response.WriteAsync(reason);
                  context.Fail(reason);
               }
            },

            // SignalR auth
            OnMessageReceived = context =>
            {
               var path = context.HttpContext.Request.Path;

               if (context.Request.Query.TryGetValue("access_token", out var accessToken) && accessToken.Count == 1
                  && !string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(ClientDefaults.SignalrEndpoint))
               {
                  context.Token = accessToken;
               }

               return Task.CompletedTask;
            }
         };
         
         options.MapInboundClaims = false;
         options.TokenValidationParameters = new TokenValidationParameters
         {
            ValidIssuer = hostingConfig.HubHostUrl,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudiences = AuthDefaults.Audiences,
            RequireAudience = true,
            ValidateLifetime = true,
            NameClaimType = "name", //ClaimTypes.Name
            RoleClaimType = "role", //ClaimTypesRole
            RequireSignedTokens = true,  
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromSeconds(5),
            IssuerSigningKey = EncryptionHelper.RsaSecurityKey,
            TokenDecryptionKey = EncryptionHelper.SymmetricSecurityKey,
         };
      });
     
      //register and configure external authentication plugins now
      var typeFinder = Singleton<ITypeFinder>.Instance;
      var externalAuthConfigurations = typeFinder.FindClassesOfType<IExternalAuthenticationRegistrar>();
      var externalAuthInstances = externalAuthConfigurations
          .Select(x => (IExternalAuthenticationRegistrar)Activator.CreateInstance(x));

      foreach (var instance in externalAuthInstances)
         instance.Configure(authenticationBuilder);
   }
 


   /// <summary>
   /// Adds an application authentication service
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   public static void AddAppAuthorization(this IServiceCollection services)
   {
      services.AddAuthorization(options =>
      {
         options.AddPolicy(AuthDefaults.AuthDevicePolicy, policy =>
         {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("scope", "Hub.Grpc");
         });
         options.AddPolicy(AuthDefaults.AuthClientPolicy, policy =>
         {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("scope", "Hub.WebAPI", "Hub.SignalR");
         });

         StandardPermissionProvider.PrepareAuthorizationPolicies(options);
      });

      services.AddRequiredScopeAuthorization();
      // Adds "scope" definition for the Scope Claim
      services.TryAddEnumerable(ServiceDescriptor.Singleton<IAuthorizationHandler, ScopeAuthorizationHandler>());
   }



   /// <summary>
   /// Add and configure MVC for the application
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   /// <returns>A builder for configuring MVC services</returns>
   public static IMvcBuilder AddAppMvc(this IServiceCollection services)
   {
      //add basic MVC feature
      var mvcBuilder = services.AddControllersWithViews();

      //https://learn.microsoft.com/ru-ru/aspnet/core/mvc/views/view-compilation?view=aspnetcore-7.0&tabs=visual-studio
      //mvcBuilder.AddRazorRuntimeCompilation();

      var appSettings = Singleton<AppSettings>.Instance;
      if (appSettings.Get<CommonConfig>().UseSessionStateTempDataProvider)
      {
         //use session-based temp data provider
         mvcBuilder.AddSessionStateTempDataProvider();
      }
      else
      {
         //use cookie-based temp data provider
         mvcBuilder.AddCookieTempDataProvider(options =>
         {
            options.Cookie.Name = $"{AppCookieDefaults.Prefix}{AppCookieDefaults.TempDataCookie}";
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
         });
      }

      services.AddRazorPages();

      //MVC now serializes JSON with camel case names by default, use this code to avoid it
      mvcBuilder.AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

      //set some options
      mvcBuilder.AddMvcOptions(options =>
      {
         //we'll use this until https://github.com/dotnet/aspnetcore/issues/6566 is solved 
         options.ModelBinderProviders.Insert(0, new InvariantNumberModelBinderProvider());
         //add custom display metadata provider 
         options.ModelMetadataDetailsProviders.Add(new AppMetadataProvider());

         //in .NET model binding for a non-nullable property may fail with an error message "The value '' is invalid"
         //here we set the locale name as the message, we'll replace it with the actual one later when not-null validation failed
         options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => AppValidationDefaults.NotNullValidationLocaleName);
      });

      //add fluent validation
      services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

      //register all available validators from Hub assemblies
      var assemblies = mvcBuilder.PartManager.ApplicationParts
          .OfType<AssemblyPart>()
          .Where(part => part.Name.StartsWith("Hub", StringComparison.InvariantCultureIgnoreCase))
          .Select(part => part.Assembly);
      services.AddValidatorsFromAssemblies(assemblies);

      //register controllers as services, it'll allow to override them
      mvcBuilder.AddControllersAsServices();

      return mvcBuilder;
   }


   /// <summary>
   /// Register custom RedirectResultExecutor
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   public static void AddAppRedirectResultExecutor(this IServiceCollection services)
   {
      //we use custom redirect executor as a workaround to allow using non-ASCII characters in redirect URLs
      services.AddScoped<IActionResultExecutor<RedirectResult>, AppRedirectResultExecutor>();
   }


   /// <summary>
   /// Add and configure MiniProfiler service
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   public static void AddAppMiniProfiler(this IServiceCollection services)
   {
      //whether database is already installed
      if (!DataSettingsManager.IsDatabaseInstalled())
         return;

      var appSettings = Singleton<AppSettings>.Instance;
      if (appSettings.Get<CommonConfig>().MiniProfilerEnabled)
         services.AddMiniProfiler(miniProfilerOptions =>
         {
            //use memory cache provider for storing each result
            ((MemoryCacheStorage)miniProfilerOptions.Storage).CacheDuration = TimeSpan.FromMinutes(appSettings.Get<CacheConfig>().DefaultCacheTime);

            //determine who can access the MiniProfiler results
            miniProfilerOptions.ResultsAuthorize = request =>
               EngineContext.Current.Resolve<IPermissionService>().AuthorizeAsync(StandardPermissionProvider.AccessProfiling).Result;

         }).AddEntityFramework();
   }


   /// <summary>
   /// Add and configure WebMarkupMin service
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   public static void AddAppWebMarkupMin(this IServiceCollection services)
   {
      //check whether database is installed
      if (!DataSettingsManager.IsDatabaseInstalled())
         return;

      services
          .AddWebMarkupMin(options =>
          {
             options.AllowMinificationInDevelopmentEnvironment = true;
             options.AllowCompressionInDevelopmentEnvironment = true;
             options.DisableMinification = !EngineContext.Current.Resolve<CommonSettings>().EnableHtmlMinification;
             options.DisableCompression = true;
             options.DisablePoweredByHttpHeaders = true;
          })
          .AddHtmlMinification(options =>
          {
             options.CssMinifierFactory = new NUglifyCssMinifierFactory();
             options.JsMinifierFactory = new NUglifyJsMinifierFactory();
          })
          .AddXmlMinification(options =>
          {
             var settings = options.MinificationSettings;
             settings.RenderEmptyTagsWithSpace = true;
             settings.CollapseTagsWithoutContent = true;
          });
   }


   /// <summary>
   /// Adds WebOptimizer to the specified <see cref="IServiceCollection"/> and enables CSS and JavaScript minification.
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   public static void AddAppWebOptimizer(this IServiceCollection services)
   {
      var optimizerConfig = Singleton<AppSettings>.Instance.Get<WebOptimizerConfig>();

      if (string.IsNullOrWhiteSpace(optimizerConfig.CacheDirectory))
      {
         var fileProvider = CommonHelper.DefaultFileProvider;
         var rootDir = fileProvider.MapPath("~/");
         var config = new WebOptimizerConfig
         {
            EnableTagHelperBundling = false,
            EnableCaching = true,
            EnableDiskCache = true,
            AllowEmptyBundle = true,
            CacheDirectory = fileProvider.Combine(rootDir, @"wwwroot\bundles")
         };

         var configs = new List<IConfig> { config };
         AppSettingsHelper.SaveAppSettings(configs, CommonHelper.DefaultFileProvider);
         Singleton<AppSettings>.Instance.Update(configs);
      }

      var cssBundling = optimizerConfig.EnableCssBundling;
      var jsBundling = optimizerConfig.EnableJavaScriptBundling;


      //add minification & bundling
      var cssSettings = new CssBundlingSettings
      {
         FingerprintUrls = false,
         Minify = cssBundling
      };

      var codeSettings = new CodeBundlingSettings
      {
         Minify = jsBundling,
         AdjustRelativePaths = false //disable this feature because it breaks function names that have "ContactUrl(" at the end
      };

      services.AddWebOptimizer(null, cssSettings, codeSettings);
   }


   /// <summary>
   /// Add and configure default HTTP clients
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   public static void AddAppHttpClients(this IServiceCollection services)
   {
      //default client
      services.AddHttpClient(AppHttpDefaults.DefaultHttpClient).WithProxy();

      //client to request the platform
      services.AddHttpClient<PlatformHttpClient>();

      //client to request tinyPlatform official site
      services.AddHttpClient<AppHttpClient>().WithProxy();

      //client to request reCAPTCHA service
      services.AddHttpClient<CaptchaHttpClient>().WithProxy();
   }


   /// <summary>
   /// Adds CORS policy for server clients
   /// </summary>
   /// <param name="services">service collection</param>
   public static void AddAppCors(this IServiceCollection services)
   {
      services.AddCors(o =>
      {
         o.AddPolicy(WebFrameworkDefaults.CorsPolicyName, policy =>
         {
            var securityConfig = Singleton<AppSettings>.Instance.Get<SecurityConfig>();

            policy.WithOrigins(securityConfig.CorsOrigins)
                  //.SetPreflightMaxAge(TimeSpan.FromMinutes(1))
                  //.SetIsOriginAllowed(host => true)
                  //.SetIsOriginAllowedToAllowWildcardSubdomains()
                  //.AllowCredentials()
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding")
                  .Build();
         });
      });
   }


   /// <summary>
   /// Adds central authentication service (from an auth sever)
   /// </summary>
   /// <param name="services">Collection of service descriptors</param>
   public static void AddCentralJWTAuthentication(this IServiceCollection services)
   {
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
      {
         var hostingConfig = EngineContext.Current.Resolve<AppSettings>().Get<HostingConfig>();

         options.Authority = hostingConfig.HubHostUrl;
         options.Events = new JwtBearerEvents
         {
            OnMessageReceived = context =>
            {
               var accessToken = context.Request.Query["access_token"];

               var path = context.HttpContext.Request.Path;
               if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(ClientDefaults.SignalrEndpoint))
               {
                  context.Token = accessToken;
               }
               return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
               var token = context.SecurityToken as JwtSecurityToken;
               if (token != null)
               {
                  if (context.Principal.Identity is ClaimsIdentity identity)
                  {
                     identity.AddClaim(new Claim("access_token", token.RawData));
                  }
               }

               return Task.CompletedTask;
            },
         };

         options.TokenValidationParameters = new TokenValidationParameters
         {
            ValidateIssuer = true,
            ValidateAudience = false,
            NameClaimType = ClaimTypes.Name,//JwtClaimTypes.ContactName,
            RoleClaimType = ClaimTypes.Role//JwtClaimTypes.Role,
         };
      });
   }


   /// <summary>
   /// Adds swagger services with custom adjusting
   /// </summary>
   /// <param name="services">Service collection</param>
   public static void AddSwagerTools(this IServiceCollection services)
   {
      var swaggerConfig = Singleton<AppSettings>.Instance.Get<SwaggerConfig>();

      if (!swaggerConfig.Enabled)
         return;

      // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
      services.AddEndpointsApiExplorer();
      services.AddSwaggerGen(options =>
      {
         var doc = Singleton<AppSettings>.Instance.Get<SwaggerConfig>().Doc;
         options.SwaggerDoc("v1", new OpenApiInfo
         {
            Version = "v1",
            Title = doc.Title,
            Description = doc.Description,
            TermsOfService = doc.TermsOfService,
            Contact = doc.Contact,
            License = doc.License
         });

         //var xmlFilename = "Hub.Web.xml";
         //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);
      });
   }

   /// <summary>
   /// Adds rate limiter for users and devices
   /// </summary>
   /// <param name="services"></param>
   /// <remarks>
   /// <see href="https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-8.0"/>
   /// </remarks>
   public static void AddAppRateLimiter(this IServiceCollection services)
   {
      var config = Singleton<AppSettings>.Instance.Get<RateLimitConfig>();

      services.AddRateLimiter(options =>
      {
         options.AddPolicy(policyName: "form", partitioner: (HttpContext httpContext) =>
         {
            var name = httpContext.User.Identity?.Name ?? httpContext.Request?.Cookies[$"{AppCookieDefaults.Prefix}{AppCookieDefaults.GuestUserCookie}"];
            if (config.FormRateLimit.Enabled && !string.IsNullOrEmpty(name))
               return getFixedLimiter(config.FormRateLimit, httpContext.Request.Method + "form" + name);

            return RateLimitPartition.GetNoLimiter("nolimit");
         })
         .AddPolicy(policyName: "grpc_read", partitioner: (HttpContext httpContext) =>
         {
            var name = httpContext.User.Identity?.Name;
            if (config.GrpcClientReadRateLimit.Enabled && !string.IsNullOrEmpty(name))
               return getFixedLimiter(config.GrpcClientReadRateLimit, "grpc_read" + name + httpContext.Request.Path.Value);

            return RateLimitPartition.GetNoLimiter("nolimit");
         })
         .AddPolicy(policyName: "grpc_modify", partitioner: (HttpContext httpContext) =>
         {
            var name = httpContext.User.Identity.Name;
            if (config.GrpcClientModifyRateLimit.Enabled && !string.IsNullOrEmpty(name))
               return getFixedLimiter(config.GrpcClientModifyRateLimit, "grpc_modify" + name + httpContext.Request.Path.Value);

            return RateLimitPartition.GetNoLimiter("nolimit");
         })
         .AddPolicy(policyName: "device", partitioner: (HttpContext httpContext) =>
         {
            var name = httpContext.User.Identity.Name;
            if (config.GrpcDeviceRateLimit.Enabled && !string.IsNullOrEmpty(name))
               return getFixedLimiter(config.GrpcDeviceRateLimit, "device" + name + httpContext.Request.Path.Value);

            return RateLimitPartition.GetNoLimiter("nolimit");
         });

         options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
         {
            var path = httpContext.Request.Path.Value;

            // rate limiter for authenticated users
            if ((httpContext.User.Identity?.IsAuthenticated ?? false) && !httpContext.User.IsInRole(UserDefaults.AdministratorsRoleName))
            {
               if (config.SignalrRateLimit.Enabled && path.StartsWith(ClientDefaults.SignalrEndpoint))
                  return getFixedLimiter(config.SignalrRateLimit, ClientDefaults.SignalrEndpoint + httpContext.User.Identity.Name);
               else if (config.IpcamRateLimit.Enabled && path.Trim('/').StartsWith(ClientDefaults.IpCamEndpoint))
                  return getFixedLimiter(config.IpcamRateLimit, ClientDefaults.IpCamEndpoint + httpContext.User.Identity.Name);
            }

            // others
            return RateLimitPartition.GetNoLimiter("FirstEntry");
         });

         options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
         options.OnRejected = async (context, token) =>
         {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
               await context.HttpContext.Response.WriteAsync(
                   $"Too many requests. Please try again after {retryAfter.TotalSeconds} second(s). " +
                   $"Read more about our rate limits at https://tinyPlat.com/docs/ratelimiting.", cancellationToken: token);
            }
            else
            {
               await context.HttpContext.Response.WriteAsync(
                   "Too many requests. Please try again later. " +
                   "Read more about our rate limits at https://tinyPlat.com/docs/ratelimiting.", cancellationToken: token);
            }
         };

         RateLimitPartition<string> getFixedLimiter(RateLimitConfig.Config config, string partitionKey) =>
            RateLimitPartition.GetFixedWindowLimiter(partitionKey: partitionKey,
               factory: partition => new FixedWindowRateLimiterOptions
               {
                  AutoReplenishment = config.AutoReplenishment,
                  PermitLimit = config.PermitLimit,
                  QueueLimit = config.QueueLimit,
                  Window = TimeSpan.FromSeconds(config.Window),
                  QueueProcessingOrder = config.QueueProcessingOrder,
               });
      });
   }
}