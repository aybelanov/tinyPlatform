using Hub.Web.Framework.Mvc.Filters;

namespace Hub.Web.Framework.Controllers
{
   /// <summary>
   /// Base controller for plugins
   /// </summary>
   [NotNullValidationMessage]
   public abstract class BasePluginController : BaseController
   {
   }
}
