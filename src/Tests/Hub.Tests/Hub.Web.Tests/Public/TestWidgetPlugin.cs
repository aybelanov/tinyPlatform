using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Services.Cms;
using Hub.Services.Plugins;

namespace Hub.Web.Tests.Public
{
   public class TestWidgetPlugin : BasePlugin, IWidgetPlugin
    {
        public bool HideInWidgetList { get; } = false;
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>{ "test widget zone" });
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return nameof(TestWidgetPlugin);
        }
    }
}
