using AutoMapper;
using Devices.Dispatcher.Configuration;
using Devices.Dispatcher.Services;
using Devices.Dispatcher.Services.Authentication;
using Devices.Dispatcher.Services.Hosted;
using Devices.Dispatcher.Services.Settings;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Shared.Devices;
using Shared.Devices.Proto;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace Devices.Dispatcher.Infrastructure;

/// <summary>
/// Service extensions
/// </summary>
public static class ServiceCollectionExtensions
{
   /// <summary>
   /// Configures writing to appsettings 
   /// </summary>
   public static void ConfigureWritable<T>(this IServiceCollection services, IConfigurationSection section, string file = "appsettings.json") where T : class, new()
   {
      services.Configure<T>(section);
      services.AddTransient<IWritableOptions<T>>(sp =>
      {
         var configuration = (IConfigurationRoot)sp.GetService<IConfiguration>();
         var environment = sp.GetService<IWebHostEnvironment>();
         var options = sp.GetService<IOptionsMonitor<T>>();
         return new WritableOptions<T>(environment, options, configuration, section.Key, file);
      });
   }

   /// <summary>
   /// Adds the device configuration
   /// </summary>
   public static void AddConnectConfiguration(this IServiceCollection services, IConfiguration configuration)
   {
      services.ConfigureWritable<HubConnections>(configuration.GetSection(nameof(HubConnections)));

      services.AddSingleton(sp =>
      {
         var connectConfiguration = new HubConnections();
         configuration.Bind(nameof(HubConnections), connectConfiguration);
         return connectConfiguration;
      });
   }

   /// <summary>
   /// Adds a hub grpc client
   /// </summary>
   public static void AddHubGrpcClient(this IServiceCollection services, IConfiguration config)
   {
      services.AddTransient<ConfigurationSyncMessageHandler>();
      services.AddTransient<AuthMessageHandler>();
     
      var hubConnections = config.GetSection(nameof(HubConnections)).Get<HubConnections>();

      // http cleint for token requests
      var authClientBuilder = services.AddHttpClient(Defaults.TokenHttpClient, (sp, c) => 
      {
         c.BaseAddress = new Uri($"{hubConnections.HubEndpoint.Trim('/')}/connect/token");
         c.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
         c.DefaultRequestHeaders.Add("X-Dispatcher-version", Defaults.ClientVersion);
      });

#if DEBUG
      if (IPAddress.TryParse(new Uri(hubConnections.HubEndpoint).Host, out var authIp) && authIp.IsIP4InternalIP())
      {
         authClientBuilder.ConfigurePrimaryHttpMessageHandler(() =>
             new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator });
      }
#endif

      // httpclient to create grpc clients
      var grpcClientBuilder = services.AddHttpClient(Defaults.HubGrpcHttpClient, (sp, c) =>
      {
         c.BaseAddress = new Uri(hubConnections.HubGrpcEndpoint.Trim('/'));
         
         // This timeout value is used for streaming methods by default.
         // It should be overridden for unary methods with deadline value
         c.Timeout = TimeSpan.FromMilliseconds(int.MaxValue);
         c.DefaultRequestHeaders.Add(DispatcherDefaults.AppVersionHeader, Defaults.ClientVersion);
      })
      .AddHttpMessageHandler<AuthMessageHandler>()
      .AddHttpMessageHandler<ConfigurationSyncMessageHandler>();

#if DEBUG
      if (IPAddress.TryParse(new Uri(hubConnections.HubGrpcEndpoint).Host, out var grpcIp) && grpcIp.IsIP4InternalIP())
      {
         grpcClientBuilder.ConfigurePrimaryHttpMessageHandler(() =>
             new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator });
      }
#endif
      
      services.AddScoped(sp =>
      {
         var hubConnections = sp.GetRequiredService<HubConnections>();
         var channel = GrpcChannel.ForAddress(
              hubConnections.HubGrpcEndpoint,
              new GrpcChannelOptions()
              {
                 HttpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(Defaults.HubGrpcHttpClient),
                 MaxReceiveMessageSize = null,
                 MaxSendMessageSize = null,
                 DisposeHttpClient = false,   
              });

         var client = new DeviceCalls.DeviceCallsClient(channel);
         return client;
      });
   }

   /// <summary>
   /// Adds Automaper
   /// </summary>
   public static IServiceCollection AddAutoMapper(this IServiceCollection services)
   {
      //create AutoMapper configuration
      var config = new MapperConfiguration(cfg => cfg.AddProfile<ProtoMapperConfiguration>());

      //register
      AutoMapperConfiguration.Init(config);

      return services;
   }


   /// <summary>
   /// Adds application and business 
   /// </summary>
   public static IServiceCollection AddAppLogicServices(this IServiceCollection services)
   {
      services.AddHostedService<ClearDBWorker>();
      services.AddHostedService<Point2PointWorker>();
      services.AddHostedService<SensorDataWorker>();
      services.AddHostedService<VideoServiceWorker>();
      services.AddSingleton<IPoint2PointService, Point2PointService>();
      services.AddScoped<ICommandService, CommandService>();
      services.AddScoped<ISettingService, SettingService>();
      services.AddSingleton<ITokenProvider, AppTokenProvider>();

      return services;
   }

   /// <summary>
   /// Adjusts the application log
   /// </summary>
   public static void LoggingAdjust(this ILoggingBuilder builder)
   {
      builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
      builder.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning);
   }

   /// <summary>
   /// Adds swagger services with custom adjusting
   /// </summary>
   /// <param name="services">Service collection</param>
   public static void AddSwagerTools(this IServiceCollection services)
   {
      services.AddEndpointsApiExplorer();
      services.AddSwaggerGen(options =>
      {
         options.SwaggerDoc("v1", new OpenApiInfo
         {
            Version = "v1",
            Title = "tinyDispatcher",
            Description = "tiny device dispatcher api of tinyPlatform",
            TermsOfService = new("https://tinyPlat.com/copyrighht"),
            Contact = new() { Url = new("https://tinyPlat.com"), Email = "info@tinyPlat.com" },
            License = new() { Url = new("https://tinyPlat.com/license") }
         });

         var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
         options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);
      });
   }
}
