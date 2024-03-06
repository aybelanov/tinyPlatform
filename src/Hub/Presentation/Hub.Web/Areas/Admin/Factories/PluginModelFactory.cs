﻿using Hub.Core;
using Hub.Core.Caching;
using Hub.Data.Extensions;
using Hub.Services.Authentication.External;
using Hub.Services.Authentication.MultiFactor;
using Hub.Services.Cms;
using Hub.Services.Localization;
using Hub.Services.Plugins;
using Hub.Services.Plugins.Marketplace;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Plugins;
using Hub.Web.Areas.Admin.Models.Plugins.Marketplace;
using Hub.Web.Framework.Factories;
using Hub.Web.Framework.Models.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the plugin model factory implementation
/// </summary>
public partial class PluginModelFactory : IPluginModelFactory
{
   #region Fields

   private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
   private readonly IAuthenticationPluginManager _authenticationPluginManager;
   private readonly IBaseAdminModelFactory _baseAdminModelFactory;
   private readonly ILocalizationService _localizationService;
   private readonly ILocalizedModelFactory _localizedModelFactory;
   private readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
   private readonly IPluginService _pluginService;
   private readonly IStaticCacheManager _staticCacheManager;
   private readonly IWidgetPluginManager _widgetPluginManager;
   private readonly IWorkContext _workContext;
   private readonly OfficialFeedManager _officialFeedManager;

   #endregion

   #region Ctor

   public PluginModelFactory(IAclSupportedModelFactory aclSupportedModelFactory,
       IAuthenticationPluginManager authenticationPluginManager,
       IBaseAdminModelFactory baseAdminModelFactory,
       ILocalizationService localizationService,
       IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
       ILocalizedModelFactory localizedModelFactory,
       IPluginService pluginService,
       IStaticCacheManager staticCacheManager,
       IWidgetPluginManager widgetPluginManager,
       IWorkContext workContext,
       OfficialFeedManager officialFeedManager)
   {
      _aclSupportedModelFactory = aclSupportedModelFactory;
      _authenticationPluginManager = authenticationPluginManager;
      _baseAdminModelFactory = baseAdminModelFactory;
      _localizationService = localizationService;
      _localizedModelFactory = localizedModelFactory;
      _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
      _pluginService = pluginService;
      _staticCacheManager = staticCacheManager;
      _widgetPluginManager = widgetPluginManager;
      _workContext = workContext;
      _officialFeedManager = officialFeedManager;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Prepare plugin model properties of the installed plugin
   /// </summary>
   /// <param name="model">Plugin model</param>
   /// <param name="plugin">Plugin</param>
   protected virtual void PrepareInstalledPluginModel(PluginModel model, IPlugin plugin)
   {
      if (model == null)
         throw new ArgumentNullException(nameof(model));

      if (plugin == null)
         throw new ArgumentNullException(nameof(plugin));

      //prepare configuration URL
      model.ConfigurationUrl = plugin.GetConfigurationPageUrl();

      //prepare enabled/disabled (only for some plugin types)
      model.CanChangeEnabled = true;
      switch (plugin)
      {
         case IExternalAuthenticationMethod externalAuthenticationMethod:
            model.IsEnabled = _authenticationPluginManager.IsPluginActive(externalAuthenticationMethod);
            break;

         case IMultiFactorAuthenticationMethod multiFactorAuthenticationMethod:
            model.IsEnabled = _multiFactorAuthenticationPluginManager.IsPluginActive(multiFactorAuthenticationMethod);
            break;

         case IWidgetPlugin widgetPlugin:
            model.IsEnabled = _widgetPluginManager.IsPluginActive(widgetPlugin);
            break;

         default:
            model.CanChangeEnabled = false;
            break;
      }
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare plugin search model
   /// </summary>
   /// <param name="searchModel">Plugin search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the plugin search model
   /// </returns>
   public virtual async Task<PluginSearchModel> PreparePluginSearchModelAsync(PluginSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare available load plugin modes
      await _baseAdminModelFactory.PrepareLoadPluginModesAsync(searchModel.AvailableLoadModes, false);

      //prepare available groups
      await _baseAdminModelFactory.PreparePluginGroupsAsync(searchModel.AvailableGroups);

      //prepare page parameters
      searchModel.SetGridPageSize();

      searchModel.NeedToRestart = _pluginService.IsRestartRequired();

      return searchModel;
   }

   /// <summary>
   /// Prepare paged plugin list model
   /// </summary>
   /// <param name="searchModel">Plugin search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the plugin list model
   /// </returns>
   public virtual async Task<PluginListModel> PreparePluginListModelAsync(PluginSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //get parameters to filter plugins
      var group = string.IsNullOrEmpty(searchModel.SearchGroup) || searchModel.SearchGroup.Equals("0") ? null : searchModel.SearchGroup;
      var loadMode = (LoadPluginsMode)searchModel.SearchLoadModeId;
      var friendlyName = string.IsNullOrEmpty(searchModel.SearchFriendlyName) ? null : searchModel.SearchFriendlyName;
      var author = string.IsNullOrEmpty(searchModel.SearchAuthor) ? null : searchModel.SearchAuthor;

      //filter visible plugins
      var plugins = (await _pluginService.GetPluginDescriptorsAsync<IPlugin>(group: group, loadMode: loadMode, friendlyName: friendlyName, author: author))
          .Where(p => p.ShowInPluginsList)
          .OrderBy(plugin => plugin.Group).ToList()
          .ToPagedList(searchModel);

      //prepare list model
      var model = await new PluginListModel().PrepareToGridAsync(searchModel, plugins, () =>
      {
         return plugins.SelectAwait(async pluginDescriptor =>
            {
               //fill in model values from the entity
               var pluginModel = pluginDescriptor.ToPluginModel<PluginModel>();

               //fill in additional values (not existing in the entity)
               pluginModel.LogoUrl = await _pluginService.GetPluginLogoUrlAsync(pluginDescriptor);

               if (pluginDescriptor.Installed)
                  PrepareInstalledPluginModel(pluginModel, pluginDescriptor.Instance<IPlugin>());

               return pluginModel;
            });
      });

      return model;
   }

   /// <summary>
   /// Prepare plugin model
   /// </summary>
   /// <param name="model">Plugin model</param>
   /// <param name="pluginDescriptor">Plugin descriptor</param>
   /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the plugin model
   /// </returns>
   public virtual async Task<PluginModel> PreparePluginModelAsync(PluginModel model, PluginDescriptor pluginDescriptor, bool excludeProperties = false)
   {
      Func<PluginLocalizedModel, long, Task> localizedModelConfiguration = null;

      if (pluginDescriptor != null)
      {
         //fill in model values from the entity
         model ??= pluginDescriptor.ToPluginModel(model);

         model.LogoUrl = await _pluginService.GetPluginLogoUrlAsync(pluginDescriptor);
         model.SelectedUserRoleIds = pluginDescriptor.LimitedToUserRoles;
         var plugin = pluginDescriptor.Instance<IPlugin>();
         if (pluginDescriptor.Installed)
            PrepareInstalledPluginModel(model, plugin);

         //define localized model configuration action
         localizedModelConfiguration = async (locale, languageId) =>
         {
            locale.FriendlyName = await _localizationService.GetLocalizedFriendlyNameAsync(plugin, languageId, false);
         };
      }

      //prepare localized models
      if (!excludeProperties)
         model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

      //prepare model user roles
      await _aclSupportedModelFactory.PrepareModelUserRolesAsync(model);

      return model;
   }

   /// <summary>
   /// Prepare search model of plugins of the official feed
   /// </summary>
   /// <param name="searchModel">Search model of plugins of the official feed</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the search model of plugins of the official feed
   /// </returns>
   public virtual async Task<OfficialFeedPluginSearchModel> PrepareOfficialFeedPluginSearchModelAsync(OfficialFeedPluginSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare available versions
      var pluginVersions = await _officialFeedManager.GetVersionsAsync();
      searchModel.AvailableVersions.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });
      foreach (var version in pluginVersions)
         searchModel.AvailableVersions.Add(new SelectListItem { Text = version.Name, Value = version.Id.ToString() });

      //pre-select current version
      //current version name and named on official site do not match. that's why we use "Contains"
      var currentVersionItem = searchModel.AvailableVersions.FirstOrDefault(x => x.Text.Contains(AppVersion.CURRENT_VERSION));
      if (currentVersionItem != null)
      {
         searchModel.SearchVersionId = int.Parse(currentVersionItem.Value);
         currentVersionItem.Selected = true;
      }

      //prepare available plugin categories
      var pluginCategories = await _officialFeedManager.GetCategoriesAsync();
      searchModel.AvailableCategories.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });
      foreach (var pluginCategory in pluginCategories)
      {
         var pluginCategoryNames = new List<string>();
         var tmpCategory = pluginCategory;
         while (tmpCategory != null)
         {
            pluginCategoryNames.Add(tmpCategory.Name);
            tmpCategory = pluginCategories.FirstOrDefault(category => category.Id == tmpCategory.ParentCategoryId);
         }

         pluginCategoryNames.Reverse();

         searchModel.AvailableCategories.Add(new SelectListItem
         {
            Value = pluginCategory.Id.ToString(),
            Text = string.Join(" >> ", pluginCategoryNames)
         });
      }

      //prepare available prices
      searchModel.AvailablePrices.Add(new SelectListItem
      {
         Value = "0",
         Text = await _localizationService.GetResourceAsync("Admin.Common.All")
      });
      searchModel.AvailablePrices.Add(new SelectListItem
      {
         Value = "10",
         Text = await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.OfficialFeed.Price.Free")
      });
      searchModel.AvailablePrices.Add(new SelectListItem
      {
         Value = "20",
         Text = await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.OfficialFeed.Price.Commercial")
      });

      //prepare page parameters
      searchModel.SetGridPageSize(15, "15");

      return searchModel;
   }

   /// <summary>
   /// Prepare paged list model of plugins of the official feed
   /// </summary>
   /// <param name="searchModel">Search model of plugins of the official feed</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the list model of plugins of the official feed
   /// </returns>
   public virtual async Task<OfficialFeedPluginListModel> PrepareOfficialFeedPluginListModelAsync(OfficialFeedPluginSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //get plugins
      var plugins = await _officialFeedManager.GetAllPluginsAsync(categoryId: searchModel.SearchCategoryId,
          versionId: searchModel.SearchVersionId,
          price: searchModel.SearchPriceId,
          searchTerm: searchModel.SearchName,
          pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

      //prepare list model
      var model = new OfficialFeedPluginListModel().PrepareToGrid(searchModel, plugins, () =>
      {
         //fill in model values from the entity
         return plugins?.Select(plugin => new OfficialFeedPluginModel
         {
            Url = plugin.Url,
            Name = plugin.Name,
            CategoryName = plugin.Category,
            SupportedVersions = plugin.SupportedVersions,
            PictureUrl = plugin.PictureUrl,
            Price = plugin.Price
         }) ?? new List<OfficialFeedPluginModel>();
      });

      return model;
   }

   /// <summary>
   /// Prepare plugin models for admin navigation
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the list of models
   /// </returns>
   public virtual async Task<IList<AdminNavigationPluginModel>> PrepareAdminNavigationPluginModelsAsync()
   {
      var user = await _workContext.GetCurrentUserAsync();
      var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(AppPluginDefaults.AdminNavigationPluginsCacheKey, user);
      return await _staticCacheManager.GetAsync(cacheKey, async () =>
      {
         //get installed plugins
         return (await _pluginService.GetPluginDescriptorsAsync<IPlugin>(LoadPluginsMode.InstalledOnly, user))
                .Where(plugin => plugin.ShowInPluginsList)
                .Select(plugin => new AdminNavigationPluginModel
                {
                   FriendlyName = plugin.FriendlyName,
                   ConfigurationUrl = plugin.Instance<IPlugin>().GetConfigurationPageUrl()
                }).Where(model => !string.IsNullOrEmpty(model.ConfigurationUrl)).ToList();
      });
   }

   #endregion
}