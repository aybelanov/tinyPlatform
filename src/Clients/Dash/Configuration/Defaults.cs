﻿using Clients.Dash.Infrastructure;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Shared.Common.Helpers;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;

namespace Clients.Dash.Configuration;

/// <summary>
/// Represents cache configuration parameters
/// </summary>
public static class Defaults
{

   /// <summary>
   /// Client app version
   /// </summary>
   /// <remarks>
   /// To get form assembly: 
   /// System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
   /// </remarks>
   public const string ClientVersion = "0.20.01-beta"; 

   /// <summary>
   /// Gets or sets the default cache time in minutes
   /// </summary>
   public static int DefaultCacheTime { get; private set; } = 15;

   /// <summary>
   /// Gets or sets the short term cache time in minutes
   /// </summary>
   public static int ShortTermCacheTime { get; private set; } = 3;
   
   /// <summary>
   /// name of http client for m2m api
   /// </summary>
   public static string ApiClientName => "HubApi";

   /// <summary>
   /// HttpClient name for Grpc clients
   /// </summary>
   public static string GrpcHttpClientName => "Grpc";

   /// <summary>
   /// SignalR connection id (for notifies)
   /// </summary>
   public static string HubConnectionIdHeaderName => "X-SignalR-ConnectionId";

   /// <summary>
   /// Inner client persist db file
   /// </summary>
   public static string SqliteDbFilename => "app.db";

   /// <summary>
   /// Supported application cultures
   /// </summary>
   public static string[] SupportedCulture => new[] { "en-EN", "ru-RU" };

   /// <summary>
   /// Page size options for grids
   /// </summary>
   public static int[] GridPageSizeOptions => new int[] { 10, 20, 30, 50, 100 };

   /// <summary>
   /// In-memory sqlite connection string
   /// </summary>
   public static string SQLiteInMemoryConnectionString => "Data Source = InMemoryDb; Mode=Memory;Cache=Shared";

   /// <summary>
   /// Api key for yandex map
   /// </summary>
   public static string YMapApiKey
   {
      get
      {
         if (string.IsNullOrWhiteSpace(_yMapApiKey))
         {
            var config = Singleton<IServiceProvider>.Instance.GetRequiredService<WebAssemblyHostConfiguration>();
            _yMapApiKey = config["YandexMap:ApiKey"];
         }
         return _yMapApiKey;
      }
   }
   private static string _yMapApiKey;

   private const string V = """
      <?xml version="1.0" encoding="UTF-8" standalone="no"?>
      <svg version="1.1" id="svg19409" width="50" height="50" viewBox="0 0 50 50" xmlns="http://www.w3.org/2000/svg" xmlns:svg="http://www.w3.org/2000/svg">
         <g id="g19415">
      	  <style>
      		  path{{
      			  fill:{0};
      			  stroke-width:1;
      		  }}
      	  </style>
          <path
             d="m 92.27759,340.71708 c -12.62338,-2.42579 -16.9574,-3.9556 -20.57988,-7.26423 -2.24773,-2.05299 -4.08678,-4.63251 -4.08678,-5.73227 0,-1.09974 -1.05,-2.78678 -2.33334,-3.74896 -1.28333,-0.96217 -3.53333,-4.10792 -5,-6.99056 -2.47074,-4.85609 -3.75956,-5.6679 -17.54194,-11.04968 -13.27251,-5.18266 -15.82607,-6.7362 -23.69963,-14.41837 -4.85338,-4.73541 -9.37886,-8.60985 -10.05661,-8.60985 -0.67775,0 -3.13311,-3.98287 -5.45636,-8.85082 -4.59795,-9.63412 -4.76409,-16.38902 -0.60115,-24.43928 3.08198,-5.95986 2.50098,-8.37018 -3.40021,-14.10597 -2.98243,-2.89883 -7.58891,-8.8706 -10.23664,-13.2706 -2.64772,-4.4 -7.61043,-11.3 -11.02824,-15.33334 -6.39435,-7.54594 -17.39201,-23.94 -19.23052,-28.66666 -2.6823,-6.896 -4.65802,-18.20294 -4.7012,-26.90476 -0.0508,-10.2408 -1.00435,-12.11976 -7.98159,-15.72783 -3.56809,-1.84513 -9.399244,-12.56379 -9.399244,-17.27741 0,-2.81107 -8.81717,-12.39738 -14.98232,-16.28927 -2.74845,-1.73501 -6.41732,-5.39996 -8.15306,-8.14432 -1.73574,-2.74436 -5.094,-7.06063 -7.46281,-9.59172 -2.36881,-2.53108 -5.39477,-7.63108 -6.724356,-11.33333 -2.36684,-6.59047 -2.84375,-9.0728 -5.39176,-28.0647 -2.30216,-17.15945 -3.66442,-24.30396 -5.1707,-27.11849 -1.98603,-3.71092 -6.74272,-18.97859 -6.76445,-21.71201 -0.0167,-2.10223 -4.12881,-12.73563 -6.78926,-17.55621 -0.72447,-1.3127 -1.86679,-5.2127 -2.53849,-8.66667 -0.67171,-3.45397 -3.09815,-14.979964 -5.3921,-25.613293 -9.57619,-44.389357 -9.99203,-51.354427 -4.05738,-67.958957 4.69581,-13.13837 11.05445,-19.89428 18.81033,-19.98554 5.63248,-0.0663 23.877166,7.26975 32.086626,12.90175 3.22469,2.21226 10.96307,6.83476 17.1964,10.27223 22.508224,12.41252 30.018154,17.61649 40.585574,28.123649 5.91127,5.877558 15.22721,14.256395 20.70208,18.619642 5.47488,4.363245 15.24614,13.159877 21.71391,19.54807 10.56601,10.436029 11.96472,12.402509 13.78077,19.374839 1.11167,4.26796 4.77092,13.92019 8.13168,21.4494 l 6.11047,13.68947 8.34505,3.98274 c 9.50949,4.53846 12.69688,4.8743 17.34651,1.82776 1.91964,-1.2578 4.76172,-1.87342 6.82656,-1.47871 2.16082,0.41307 5.1437,-0.30307 7.6642,-1.84004 3.61058,-2.20171 6.92426,-2.51695 26.50269,-2.52131 22.74621,-0.005 30.17032,1.28886 33.64289,5.8635 0.74381,0.97986 3.19653,2.18662 5.45049,2.68168 4.0898,0.89826 4.1093,0.87469 9.61195,-11.62307 3.03261,-6.88773 6.40864,-15.9585 7.50228,-20.15726 1.77656,-6.82068 3.24141,-8.87166 13.748,-19.24899 6.46777,-6.388201 16.23903,-15.184833 21.7139,-19.548079 5.47488,-4.363247 14.79082,-12.742084 20.70208,-18.619641 10.56743,-10.507157 18.07735,-15.711134 40.58558,-28.12365 6.23333,-3.43747 13.97171,-8.05997 17.19639,-10.27223 8.20946,-5.63199 26.45415,-12.96803 32.08663,-12.90175 7.75588,0.0913 14.11452,6.84717 18.81033,19.98554 5.93466,16.604525 5.51882,23.569596 -4.05738,67.95896 -2.29395,10.63334 -4.72039,22.15932 -5.3921,25.61329 -0.67169,3.45397 -1.81401,7.35397 -2.53848,8.66666 -2.66045,4.82059 -6.77256,15.45399 -6.78926,17.55622 -0.0217,2.73342 -4.77842,18.00109 -6.76446,21.71201 -1.50628,2.81453 -2.86853,9.95904 -5.1707,27.11849 -2.548,18.9919 -3.02491,21.47423 -5.39175,28.0647 -1.3296,3.70225 -4.35556,8.80225 -6.72436,11.33333 -2.36881,2.53109 -5.72708,6.84736 -7.46281,9.59172 -1.73575,2.74436 -5.40462,6.40931 -8.15307,8.14432 -6.16514,3.89189 -14.98232,13.4782 -14.98232,16.28927 0,4.71362 -5.83114,15.43228 -9.39924,17.27741 -6.97724,3.60807 -7.93077,5.48703 -7.98158,15.72783 -0.0432,8.70182 -2.0189,20.00876 -4.7012,26.90476 -1.83851,4.72666 -12.83618,21.12072 -19.23052,28.66666 -3.41782,4.03334 -8.38052,10.93334 -11.02824,15.33334 -2.64774,4.4 -7.25422,10.37177 -10.23664,13.2706 -5.90119,5.73579 -6.48219,8.14611 -3.40021,14.10597 4.16294,8.05026 3.9968,14.80516 -0.60115,24.43928 -2.32325,4.86795 -4.77861,8.85082 -5.45636,8.85082 -0.67775,0 -5.20323,3.87444 -10.05661,8.60985 -7.89064,7.69884 -10.39834,9.22116 -23.69963,14.38716 -14.16388,5.50101 -15.00281,6.0434 -17.54195,11.34116 -3.9136,8.16552 -11.19218,11.36315 -26.66666,11.71519 -6.6,0.15016 -13.2,0.0424 -14.66667,-0.23944 z"
             id="path427"
             transform="matrix(0.10412584,0,0,0.10412584,15.00581,15.937268)" />
        </g>
      </svg>
      """;

   /// <summary>
   /// Logo template
   /// </summary>
   public static string LogoSvgTemplate = V;

   /// <summary>
   /// "Show all devices" settng key
   /// </summary>
   public static string ShowAllDevicesSettingKey => "ShowAllDeviceSettingKey";

   /// <summary>
   /// "Show all monitors" settng key
   /// </summary>
   public static string ShowAllMonitorsSettingKey => "ShowAllMonitorsSettingKey";

   /// <summary>
   /// "Show all widgets" settng key
   /// </summary>
   public static string ShowAllWidgetsSettingKey => "ShowAllWidgetsSettingKey";

   /// <summary>
   /// Enable admin mode
   /// </summary>
   public static string IsAdminModeEnabled => "IsAdminModeEnabled";

   /// <summary>
   /// Application theme
   /// </summary>
   public static string Theme
   {
      get
      {
         if (string.IsNullOrWhiteSpace(_theme))
         {
            using var scope = Singleton<IServiceProvider>.Instance.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var js = (IJSInProcessRuntime)scope.ServiceProvider.GetRequiredService<IJSRuntime>();
            _theme = js.Invoke<string>("appjs.getTheme");
         }
         return _theme;
      }
      set
      {
         if (_theme != value)
         {
            using var scope = Singleton<IServiceProvider>.Instance.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var js = (IJSInProcessRuntime)scope.ServiceProvider.GetRequiredService<IJSRuntime>();
            js.InvokeVoid("appjs.setTheme", value);
            _theme = value;
         }
      }
   }
   private static string _theme;

   /// <summary>
   /// Application culture
   /// </summary>
   public static string Culture
   {
      get
      {
         if (string.IsNullOrWhiteSpace(_culture))
         {
            using var scope = Singleton<IServiceProvider>.Instance.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var js = (IJSInProcessRuntime)scope.ServiceProvider.GetRequiredService<IJSRuntime>();
            _culture = js.Invoke<string>("appjs.getCulture");
         }
         return _culture;
      }
      set
      {
         if (_culture != value)
         {
            using var scope = Singleton<IServiceProvider>.Instance.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var js = (IJSInProcessRuntime)scope.ServiceProvider.GetRequiredService<IJSRuntime>();
            js.InvokeVoid("appjs.setCulture", value);
            _culture = value;
         }
      }
   }
   private static string _culture;

   /// <summary>
   /// Browser language
   /// </summary>
   public static string Language
   {
      get
      {
         if (string.IsNullOrWhiteSpace(_language))
         {
            using var scope = Singleton<IServiceProvider>.Instance.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var js = (IJSInProcessRuntime)scope.ServiceProvider.GetRequiredService<IJSRuntime>();
            _language = js.Invoke<string>("appjs.getLanguage");
         }
         return _language;
      }
   }
   private static string _language;


   /// <summary>
   /// Browser time offset
   /// </summary>
   public static TimeSpan BrowserTimeOffset
   {
      get
      {
         if (_browserTimeOffset is null)
         {
            using var scope = Singleton<IServiceProvider>.Instance.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var js = (IJSInProcessRuntime)scope.ServiceProvider.GetRequiredService<IJSRuntime>();
            var offset = js.Invoke<int>("appjs.getTimeZoneOffset");
            _browserTimeOffset = TimeSpan.FromMinutes(-offset);
         }
         return _browserTimeOffset.Value;
      }
   }
   private static TimeSpan? _browserTimeOffset;

   /// <summary>
   /// Token key in browser seeion storage
   /// </summary>
   public static string TokenKey { get; internal set; }

}