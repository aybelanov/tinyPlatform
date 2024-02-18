using System.Collections.Generic;
using Hub.Web.Tests.Public;
using Hub.Core;
using Hub.Core.Infrastructure;
using Hub.Services.Plugins;
using Microsoft.AspNetCore.Hosting;
using Moq;
using NUnit.Framework;
using Hub.Tests;

namespace Hub.Web.Tests
{
   [TestFixture]
    public abstract class WebTest : BaseAppTest
    {
        protected WebTest()
        {
            //init plugins
            InitPlugins();
        }

        private void InitPlugins()
        {
            var webHostEnvironment = new Mock<IWebHostEnvironment>();
            webHostEnvironment.Setup(x => x.ContentRootPath).Returns(System.Reflection.Assembly.GetExecutingAssembly().Location);
            webHostEnvironment.Setup(x => x.WebRootPath).Returns(System.IO.Directory.GetCurrentDirectory());
            CommonHelper.DefaultFileProvider = new AppFileProvider(webHostEnvironment.Object);

            Singleton<IPluginsInfo>.Instance = new PluginsInfo(CommonHelper.DefaultFileProvider)
            {
                PluginDescriptors = new List<PluginDescriptor>
                {
                    new PluginDescriptor(typeof(TestWidgetPlugin).Assembly)
                    {
                        PluginType = typeof(TestWidgetPlugin),
                        SystemName = "TestWidgetPlugin",
                        FriendlyName = "Test widget plugin",
                        Installed = true
                    }
                }
            };
        }
    }
}
