using System;
using System.Collections.Generic;
using Hub.Services.Tests.Directory;
using Hub.Core;
using Hub.Core.Infrastructure;
using Hub.Data.Configuration;
using Hub.Services.Plugins;
using Microsoft.AspNetCore.Hosting;
using Moq;
using NUnit.Framework;
using Hub.Tests;

namespace Hub.Services.Tests;

[TestFixture]
public abstract class ServiceTest : BaseAppTest
{
   protected ServiceTest()
   {
      //init plugins
      InitPlugins();
   }

   private static void InitPlugins()
   {
      var webHostEnvironment = new Mock<IWebHostEnvironment>();
      webHostEnvironment.Setup(x => x.ContentRootPath).Returns(System.Reflection.Assembly.GetExecutingAssembly().Location);
      webHostEnvironment.Setup(x => x.WebRootPath).Returns(System.IO.Directory.GetCurrentDirectory());
      CommonHelper.DefaultFileProvider = new AppFileProvider(webHostEnvironment.Object);

      Environment.SetEnvironmentVariable("ConnectionStrings", Singleton<DataConfig>.Instance.ConnectionString);

      Singleton<IPluginsInfo>.Instance = new PluginsInfo(CommonHelper.DefaultFileProvider)
      {
         PluginDescriptors = new List<PluginDescriptor>
             {
                 new PluginDescriptor(typeof(TestExchangeRateProvider).Assembly)
                 {
                     PluginType = typeof(TestExchangeRateProvider),
                     SystemName = "CurrencyExchange.TestProvider",
                     FriendlyName = "Test exchange rate provider",
                     Installed = true
                 }
             }
      };
   }
}
