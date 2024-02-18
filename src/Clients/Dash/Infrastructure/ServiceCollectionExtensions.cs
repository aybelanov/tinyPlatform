using AutoMapper;
using Clients.Dash.Caching;
using Clients.Dash.Configuration;
using Clients.Dash.Infrastructure.AutoMapper;
using Clients.Dash.Services.Authentication;
using Clients.Dash.Services.Configuration;
using Clients.Dash.Services.Connection;
using Clients.Dash.Services.EntityServices;
using Clients.Dash.Services.EntityServices.Grpc;
using Clients.Dash.Services.ErrorServices;
using Clients.Dash.Services.Helpers;
using Clients.Dash.Services.Localization;
using Clients.Dash.Services.Security;
using Clients.Dash.Services.UI;
using Clients.Dash.Shared.Communication;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using Shared.Clients.Configuration;
using Shared.Clients.Proto;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Clients.Dash.Infrastructure;

/// <summary>
/// Represents service collcetion extensions
/// </summary>
public static class ServiceCollectionExtensions
{
   /// <summary>
   /// Configure app infrastructure
   /// </summary>
   /// <param name="builder"></param>
   public static void PrepareAppConfiguration(this WebAssemblyHostBuilder builder)
   {
      builder.Services.AddSingleton(sp => builder.Configuration);

      var endpoinst = new ServerConfig();
      builder.Configuration.Bind("ServerConfig", endpoinst);

      Singleton<ServerConfig>.Instance = endpoinst;
   }

   /// <summary>
   /// Adds memory cachemanager
   /// </summary>
   /// <param name="services">Service collection</param>
   /// <returns>Service collection</returns>
   public static IServiceCollection AddCaching(this IServiceCollection services)
   {
      services.AddMemoryCache();
      services.AddScoped<IStaticCacheManager, MemoryCacheManager>();

      return services;
   }


   /// <summary>
   /// Adds authentication services
   /// </summary>
   /// <param name="services">Service collection</param>
   /// <returns>Service collection</returns>
   public static IServiceCollection AddAuthentication(this IServiceCollection services)
   {
      var configuration = services.BuildServiceProvider().GetRequiredService<WebAssemblyHostConfiguration>();

      services.AddOidcAuthentication(o =>
      {
         o.UserOptions.NameClaim = "name";
         o.UserOptions.RoleClaim = "role";
         o.UserOptions.ScopeClaim = "scope";

         // Configure your authentication provider options here.
         // For more information, see https://aka.ms/blazor-standalone-auth
         //builder.Configuration.Bind("Local", o.ProviderOptions);
         configuration.Bind("OpenId", o.ProviderOptions);

      })
      .AddAccountClaimsPrincipalFactory<CustomUserFactory>();
      //.AddAccountClaimsPrincipalFactory<OfflineAccountClaimsPrincipalFactory>();

      //services.AddScoped<AccountClaimsPrincipalFactory<RemoteUserAccount>, OfflineAccountClaimsPrincipalFactory>();

      services.AddOptions();
      services.AddAuthorizationCore(PermissionProvider.PreparePolicies);
      services.AddCascadingAuthenticationState();

      return services;
   }

   /// <summary>
   /// Adds SignalR client connection
   /// </summary>
   /// <param name="services">Service collection</param>
   /// <returns></returns>
   public static IServiceCollection AddSignalrClientConnection(this IServiceCollection services)
   {
      services.AddSingleton(sp =>
      {
         var hubConnection = new HubConnectionBuilder()
         .WithAutomaticReconnect(new ConnectionRetryPolicy())
         .WithUrl(Singleton<ServerConfig>.Instance.SignalrEndpoint, options =>
         {
            options.DefaultTransferFormat = Microsoft.AspNetCore.Connections.TransferFormat.Binary;
            //options.WebSocketConfiguration = config => {  };
            options.Transports = HttpTransportType.WebSockets;
            options.AccessTokenProvider = async () =>
            {
               using var scope = sp.CreateScope();
               var tokenProvider = scope.ServiceProvider.GetRequiredService<IAccessTokenProvider>();
               var tokenResult = await tokenProvider.RequestAccessToken();
               tokenResult.TryGetToken(out var token);
               return token?.Value;
            };
         })
         .AddMessagePackProtocol()
         .Build();

         return hubConnection;
      });

      services.AddSingleton<HubService>();

      return services;
   }

   /// <summary>
   /// Adds Radzen services
   /// </summary>
   /// <param name="services">Service collection</param>
   /// <returns>Service collection</returns>
   public static IServiceCollection AddRadzenServices(this IServiceCollection services)
   {
      services.AddScoped<DialogService>();
      services.AddScoped<NotificationService>();
      services.AddScoped<TooltipService>();
      services.AddScoped<ContextMenuService>();

      return services;
   }

   /// <summary>
   /// Adds authorized http client
   /// </summary>
   /// <param name="services">Service collection</param>
   /// <returns>Service collection</returns>
   public static IServiceCollection AddAuthorizedHttpClient(this IServiceCollection services)
   {
      services.AddTransient<DataLoadErrorHandler>();
      services.AddTransient<CommonHttpHandler>();
      services.AddTransient(typeof(AuthorizationMessageHandler), sp =>
      {
         var tokenProvider = sp.GetRequiredService<IAccessTokenProvider>();
         var navmanager = sp.GetRequiredService<NavigationManager>();
         var handler = new AuthorizationMessageHandler(tokenProvider, navmanager);

         handler.ConfigureHandler(authorizedUrls: new[]
         {
                Singleton<ServerConfig>.Instance.WebapiEndpoint,
                Singleton<ServerConfig>.Instance.SignalrEndpoint,
                Singleton<ServerConfig>.Instance.GrpcHost
         });

         return handler;
      });


      services.AddHttpClient(Defaults.ApiClientName, c =>
      {
         c.BaseAddress = new Uri(Singleton<ServerConfig>.Instance.WebapiEndpoint);
         c.DefaultRequestHeaders.Clear();
         c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         c.DefaultRequestHeaders.Add(ClientsDefaults.ClientVersionHeader, Defaults.ClientVersion);
         c.DefaultRequestHeaders.AcceptLanguage.Clear();
         c.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(Defaults.Culture));
      })
      .AddHttpMessageHandler<AuthorizationMessageHandler>()
      .AddHttpMessageHandler<CommonHttpHandler>()
      .AddHttpMessageHandler<DataLoadErrorHandler>();

      // for to manage life cicle of http clients by http client factory
      services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(Defaults.ApiClientName));

      return services;
   }


   /// <summary>
   /// Grpc client for db synchronization
   /// </summary>
   /// <param name="services"></param>
   public static void AddGrpcClients(this IServiceCollection services)
   {
      services.AddHttpClient(Defaults.GrpcHttpClientName, c =>
      {
         c.BaseAddress = new Uri(Singleton<ServerConfig>.Instance.GrpcHost);
         c.DefaultRequestHeaders.Clear();
         c.DefaultRequestHeaders.Add(ClientsDefaults.ClientVersionHeader, Defaults.ClientVersion);
         c.DefaultRequestHeaders.AcceptLanguage.Clear();
         c.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(Defaults.Culture));
      })
      .AddHttpMessageHandler<AuthorizationMessageHandler>()
      .AddHttpMessageHandler<CommonHttpHandler>()
      .AddHttpMessageHandler(() => new GrpcWebHandler(GrpcWebMode.GrpcWeb))
      .AddHttpMessageHandler<DataLoadErrorHandler>();

      services.AddScoped(sp => new CommonRpc.CommonRpcClient(GetChannel(sp)));
      services.AddScoped(sp => new DeviceRpc.DeviceRpcClient(GetChannel(sp)));
      services.AddScoped(sp => new MonitorRpc.MonitorRpcClient(GetChannel(sp)));
      services.AddScoped(sp => new SensorRpc.SensorRpcClient(GetChannel(sp)));
      services.AddScoped(sp => new WidgetRpc.WidgetRpcClient(GetChannel(sp)));
      services.AddScoped(sp => new PresentationRpc.PresentationRpcClient(GetChannel(sp)));
      services.AddScoped(sp => new SensorRecordRpc.SensorRecordRpcClient(GetChannel(sp)));
      services.AddScoped(sp => new DownloadTaskRpc.DownloadTaskRpcClient(GetChannel(sp)));

      GrpcChannel GetChannel(IServiceProvider sp)
      {
         var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(Defaults.GrpcHttpClientName);
         var channel = GrpcChannel.ForAddress(Singleton<ServerConfig>.Instance.GrpcHost, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null });
         return channel;
      }
   }


   /// <summary>
   /// Adds services of application logic
   /// </summary>
   /// <param name="services">Service collection</param>
   /// <returns>Service collection</returns>
   public static IServiceCollection AddApplicationServices(this IServiceCollection services)
   {
      services.AddSingleton<DataLoadProcess>();
      services.AddSingleton<SettingsService>();
      services.AddScoped<PermissionService>();
      services.AddScoped<MenuService>();
      services.AddScoped<ErrorService>();
      services.AddScoped<IHelperService, HelperService>();
      services.AddScoped<IDeviceService, DeviceGrpcService>();
      services.AddScoped<ISensorService, SensorGrpcService>();
      services.AddScoped<IMonitorService, MonitorGrpcService>();
      services.AddScoped<IWidgetService, WidgetGrpcService>();
      services.AddScoped<IPresentationService, PresentationGrpcService>();
      services.AddScoped<ISensorRecordService, SensorRecordGrpcService>();
      services.AddScoped<IDownloadTaskService, DownloadTaskGrpcService>();
      services.AddScoped<ICommonService, CommonGrpcService>();
      services.AddScoped<ClearCacheService>();

      return services;
   }


   /// <summary>
   /// Adds localization
   /// </summary>
   /// <param name="services">Service collection</param>
   /// <returns>Service collection</returns>
   public static IServiceCollection AddAppLocalization(this IServiceCollection services)
   {
      services.AddLocalization();
      services.AddScoped<Localizer>();

      return services;
   }


   /// <summary>
   /// Adds Automaper
   /// </summary>
   /// <param name="services">Service collection</param>
   /// <returns>Service collection</returns>
   public static IServiceCollection AddAutoMapper(this IServiceCollection services)
   {
      //create AutoMapper configuration
      var config = new MapperConfiguration(cfg =>
      {
         cfg.AddProfile<ModelMapperConfiguration>();
      });

      //register
      AutoMapperConfiguration.Init(config);

      return services;
   }

   /// <summary>
   /// Inizializes culture
   /// </summary>
   /// <param name="host">WebAssembly host</param>
   /// <returns></returns>
   public static void InitializeGlobalVariables(this WebAssemblyHost host)
   {
      Singleton<IServiceProvider>.Instance = host.Services;
      
      CultureInfo.DefaultThreadCurrentCulture =
      CultureInfo.DefaultThreadCurrentUICulture =
      Defaults.Culture != null
         ? new CultureInfo(Defaults.Culture)
         : (Defaults.Language != null && Defaults.SupportedCulture.Any(x => x.Equals(Defaults.Language, StringComparison.InvariantCultureIgnoreCase)))
           ? new CultureInfo(Defaults.Language)
           : new CultureInfo("en-US");

      Defaults.Culture = CultureInfo.DefaultThreadCurrentCulture.Name;

      Defaults.TokenKey = host.Configuration["OpenId:Authority"].Trim('/') + "/:" + typeof(Program).Assembly.GetName().Name;
   }
}