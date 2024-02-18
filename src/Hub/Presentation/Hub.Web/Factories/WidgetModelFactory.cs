using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Web.Models.Cms;
using Microsoft.AspNetCore.Routing;
using Hub.Core;
using Hub.Core.Caching;
using Hub.Services.Cms;
using Hub.Services.Users;
using Hub.Web.Framework.Themes;
using Hub.Web.Infrastructure.Cache;

namespace Hub.Web.Factories;

/// <summary>
/// Represents the widget model factory
/// </summary>
public partial class WidgetModelFactory : IWidgetModelFactory
{
   #region Fields

   private readonly IUserService _userService;
   private readonly IStaticCacheManager _staticCacheManager;
   private readonly IThemeContext _themeContext;
   private readonly IWidgetPluginManager _widgetPluginManager;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   public WidgetModelFactory(IUserService userService,
       IStaticCacheManager staticCacheManager,
       IThemeContext themeContext,
       IWidgetPluginManager widgetPluginManager,
       IWorkContext workContext)
   {
      _userService = userService;
      _staticCacheManager = staticCacheManager;
      _themeContext = themeContext;
      _widgetPluginManager = widgetPluginManager;
      _workContext = workContext;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Get the render widget models
   /// </summary>
   /// <param name="widgetZone">Name of widget zone</param>
   /// <param name="additionalData">Additional data object</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the list of the render widget models
   /// </returns>
   public virtual async Task<List<RenderWidgetModel>> PrepareRenderWidgetModelAsync(string widgetZone, object additionalData = null)
   {
      var theme = await _themeContext.GetWorkingThemeNameAsync();
      var user = await _workContext.GetCurrentUserAsync();
      var userRoleIds = await _userService.GetUserRoleIdsAsync(user);

      var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(AppModelCacheDefaults.WidgetModelKey, userRoleIds, widgetZone, theme);

      var cachedModels = await _staticCacheManager.GetAsync(cacheKey, async () =>
          (await _widgetPluginManager.LoadActivePluginsAsync(user, widgetZone))
          .Select(widget => new RenderWidgetModel
          {
             WidgetViewComponentName = widget.GetWidgetViewComponentName(widgetZone),
             WidgetViewComponentArguments = new RouteValueDictionary { ["widgetZone"] = widgetZone }
          }));

      //"WidgetViewComponentArguments" property of widget models depends on "additionalData".
      //We need to clone the cached model before modifications (the updated one should not be cached)
      var models = cachedModels.Select(renderModel => new RenderWidgetModel
      {
         WidgetViewComponentName = renderModel.WidgetViewComponentName,
         WidgetViewComponentArguments = new RouteValueDictionary { ["widgetZone"] = widgetZone, ["additionalData"] = additionalData }
      }).ToList();

      return models;
   }

   #endregion
}