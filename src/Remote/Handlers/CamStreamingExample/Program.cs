using CamStreamingExample;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

IHost host = Host.CreateDefaultBuilder(args).UseSystemd().ConfigureServices(services =>
{
   var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
   services.AddHttpClient("default").ConfigureHttpClient(c => c.BaseAddress = new Uri(config["DispatcherUrl"]));
   services.AddHostedService<Worker>();

}).Build();

await host.RunAsync();