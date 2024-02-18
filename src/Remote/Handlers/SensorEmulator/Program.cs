using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SensorEmulator;
using System;

IHost host = Host.CreateDefaultBuilder(args).UseSystemd()
   .ConfigureAppConfiguration(c => c.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true))
   .ConfigureServices(services =>
   {
      var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
      services.AddHttpClient("default").ConfigureHttpClient(c => c.BaseAddress = new Uri(config["DispatcherApiUrl"]));
      services.AddHostedService<Worker>();

   }).Build();


await host.RunAsync();