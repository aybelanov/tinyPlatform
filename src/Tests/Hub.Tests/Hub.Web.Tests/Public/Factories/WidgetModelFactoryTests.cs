using Hub.Core.Domain.Cms;
using Hub.Services.Configuration;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Hub.Web.Tests.Public.Factories;

[TestFixture]
public class WidgetModelFactoryTests : WebTest
{
   private ISettingService _settingsService;

   [OneTimeSetUp]
   public async Task SetUp()
   {
      _settingsService = GetService<ISettingService>();

      var widgetSettings = GetService<WidgetSettings>();

      widgetSettings.ActiveWidgetSystemNames.Add("TestWidgetPlugin");

      await _settingsService.SaveSettingAsync(widgetSettings);
   }

   [OneTimeTearDown]
   public async Task TearDown()
   {
      var widgetSettings = GetService<WidgetSettings>();

      widgetSettings.ActiveWidgetSystemNames.Remove("TestWidgetPlugin");

      await _settingsService.SaveSettingAsync(widgetSettings);
   }
}
