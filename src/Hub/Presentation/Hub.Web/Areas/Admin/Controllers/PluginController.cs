using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Cms;
using Hub.Core.Domain.Users;
using Hub.Core.Events;
using Hub.Data.Extensions;
using Hub.Services.Authentication.External;
using Hub.Services.Authentication.MultiFactor;
using Hub.Services.Cms;
using Hub.Services.Configuration;
using Hub.Services.Localization;
using Hub.Services.Logging;
using Hub.Services.Messages;
using Hub.Services.Plugins;
using Hub.Services.Security;
using Hub.Services.Themes;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Models.Plugins;
using Hub.Web.Areas.Admin.Models.Plugins.Marketplace;
using Hub.Web.Framework.Controllers;
using Hub.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class PluginController : BaseAdminController
{
   #region Fields

   private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
   private readonly IAuthenticationPluginManager _authenticationPluginManager;
   private readonly IUserActivityService _userActivityService;
   private readonly IEventPublisher _eventPublisher;
   private readonly ILocalizationService _localizationService;
   private readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
   private readonly INotificationService _notificationService;
   private readonly IPermissionService _permissionService;
   private readonly IPluginModelFactory _pluginModelFactory;
   private readonly IPluginService _pluginService;
   private readonly ISettingService _settingService;
   private readonly IUploadService _uploadService;
   private readonly IWebHelper _webHelper;
   private readonly IWidgetPluginManager _widgetPluginManager;
   private readonly IWorkContext _workContext;
   private readonly MultiFactorAuthenticationSettings _multiFactorAuthenticationSettings;
   private readonly WidgetSettings _widgetSettings;

   #endregion

   #region Ctor

   public PluginController(ExternalAuthenticationSettings externalAuthenticationSettings,
       IAuthenticationPluginManager authenticationPluginManager,
       IUserActivityService userActivityService,
       IEventPublisher eventPublisher,
       ILocalizationService localizationService,
       IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
       INotificationService notificationService,
       IPermissionService permissionService,
       IPluginModelFactory pluginModelFactory,
       IPluginService pluginService,
       ISettingService settingService,
       IUploadService uploadService,
       IWebHelper webHelper,
       IWidgetPluginManager widgetPluginManager,
       IWorkContext workContext,
       MultiFactorAuthenticationSettings multiFactorAuthenticationSettings,
       WidgetSettings widgetSettings)
   {
      _externalAuthenticationSettings = externalAuthenticationSettings;
      _authenticationPluginManager = authenticationPluginManager;
      _userActivityService = userActivityService;
      _eventPublisher = eventPublisher;
      _localizationService = localizationService;
      _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
      _notificationService = notificationService;
      _permissionService = permissionService;
      _pluginModelFactory = pluginModelFactory;
      _pluginService = pluginService;
      _settingService = settingService;
      _uploadService = uploadService;
      _webHelper = webHelper;
      _widgetPluginManager = widgetPluginManager;
      _workContext = workContext;
      _multiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
      _widgetSettings = widgetSettings;
   }

   #endregion

   #region Methods

   public virtual async Task<IActionResult> List()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
         return AccessDeniedView();

      var model = await _pluginModelFactory.PreparePluginSearchModelAsync(new PluginSearchModel());

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> ListSelect(PluginSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _pluginModelFactory.PreparePluginListModelAsync(searchModel);

      return Json(model);
   }

   [SaveLastVisitedPage(true)]
   public virtual async Task<IActionResult> AdminNavigationPlugins()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
         return Json(new List<string>());

      //prepare models
      var models = await (await _pluginModelFactory.PrepareAdminNavigationPluginModelsAsync()).SelectAwait(async model => new
      {
         title = model.FriendlyName,
         link = model.ConfigurationUrl,
         parent = await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.Local"),
         grandParent = string.Empty,
         rate = -50 //negative rate is set to move plugins to the end of list
      }).ToListAsync();

      return Json(models);
   }

   [HttpPost]
   public virtual async Task<IActionResult> UploadPluginsAndThemes(IFormFile archivefile)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
         return AccessDeniedView();

      try
      {
         if (archivefile == null || archivefile.Length == 0)
            throw new AppException(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));

         var descriptors = _uploadService.UploadPluginsAndThemes(archivefile);
         var pluginDescriptors = descriptors.OfType<PluginDescriptor>().ToList();
         var themeDescriptors = descriptors.OfType<ThemeDescriptor>().ToList();

         //activity log
         foreach (var descriptor in pluginDescriptors)
            await _userActivityService.InsertActivityAsync("UploadNewPlugin",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.UploadNewPlugin"), descriptor.FriendlyName));

         foreach (var descriptor in themeDescriptors)
            await _userActivityService.InsertActivityAsync("UploadNewTheme",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.UploadNewTheme"), descriptor.FriendlyName));

         //events
         if (pluginDescriptors.Any())
            await _eventPublisher.PublishAsync(new PluginsUploadedEvent(pluginDescriptors));

         if (themeDescriptors.Any())
            await _eventPublisher.PublishAsync(new ThemesUploadedEvent(themeDescriptors));

         var message = string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.Uploaded"), pluginDescriptors.Count, themeDescriptors.Count);
         _notificationService.SuccessNotification(message);

         return View("RestartApplication", Url.Action("List", "Plugin"));
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
      }

      return RedirectToAction("List");
   }

   [HttpPost, ActionName("List")]
   [FormValueRequired(FormValueRequirement.StartsWith, "install-plugin-link-")]
   public virtual async Task<IActionResult> Install(IFormCollection form)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
         return AccessDeniedView();

      try
      {
         //get plugin system name
         string systemName = null;
         foreach (var formValue in form.Keys)
            if (formValue.StartsWith("install-plugin-link-", StringComparison.InvariantCultureIgnoreCase))
               systemName = formValue["install-plugin-link-".Length..];

         var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.All);
         if (pluginDescriptor == null)
            //No plugin found with the specified id
            return RedirectToAction("List");

         //check whether plugin is not installed
         if (pluginDescriptor.Installed)
            return RedirectToAction("List");

         await _pluginService.PreparePluginToInstallAsync(pluginDescriptor.SystemName, await _workContext.GetCurrentUserAsync());
         pluginDescriptor.ShowInPluginsList = false;
         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.ChangesApplyAfterReboot"));
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
      }

      return RedirectToAction("List");
   }

   [HttpPost, ActionName("List")]
   [FormValueRequired(FormValueRequirement.StartsWith, "uninstall-plugin-link-")]
   public virtual async Task<IActionResult> Uninstall(IFormCollection form)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
         return AccessDeniedView();

      try
      {
         //get plugin system name
         string systemName = null;
         foreach (var formValue in form.Keys)
            if (formValue.StartsWith("uninstall-plugin-link-", StringComparison.InvariantCultureIgnoreCase))
               systemName = formValue["uninstall-plugin-link-".Length..];

         var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.All);
         if (pluginDescriptor == null)
            //No plugin found with the specified id
            return RedirectToAction("List");

         //check whether plugin is installed
         if (!pluginDescriptor.Installed)
            return RedirectToAction("List");

         await _pluginService.PreparePluginToUninstallAsync(pluginDescriptor.SystemName);
         pluginDescriptor.ShowInPluginsList = false;
         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.ChangesApplyAfterReboot"));
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
      }

      return RedirectToAction("List");
   }

   [HttpPost, ActionName("List")]
   [FormValueRequired(FormValueRequirement.StartsWith, "delete-plugin-link-")]
   public virtual async Task<IActionResult> Delete(IFormCollection form)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
         return AccessDeniedView();

      try
      {
         //get plugin system name
         string systemName = null;
         foreach (var formValue in form.Keys)
            if (formValue.StartsWith("delete-plugin-link-", StringComparison.InvariantCultureIgnoreCase))
               systemName = formValue["delete-plugin-link-".Length..];

         var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.All);

         //check whether plugin is not installed
         if (pluginDescriptor.Installed)
            return RedirectToAction("List");

         await _pluginService.PreparePluginToDeleteAsync(pluginDescriptor.SystemName);
         pluginDescriptor.ShowInPluginsList = false;
         _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.ChangesApplyAfterReboot"));
      }
      catch (Exception exc)
      {
         await _notificationService.ErrorNotificationAsync(exc);
      }

      return RedirectToAction("List");
   }

   [HttpPost, ActionName("List")]
   [FormValueRequired("plugin-reload-grid")]
   public virtual async Task<IActionResult> ReloadList()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
         return AccessDeniedView();

      await _pluginService.UninstallPluginsAsync();
      await _pluginService.DeletePluginsAsync();

      return View("RestartApplication", Url.Action("List", "Plugin"));
   }

   public virtual async Task<IActionResult> UninstallAndDeleteUnusedPlugins(string[] names)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
         return AccessDeniedView();

      foreach (var name in names)
         await _pluginService.PreparePluginToUninstallAsync(name);

      await _pluginService.UninstallPluginsAsync();

      foreach (var name in names)
         await _pluginService.PreparePluginToDeleteAsync(name);

      await _pluginService.DeletePluginsAsync();

      return View("RestartApplication", Url.Action("Warnings", "Common"));
   }

   [HttpPost, ActionName("List")]
   [FormValueRequired("plugin-apply-changes")]
   public virtual async Task<IActionResult> ApplyChanges()
   {
      return await ReloadList();
   }

   [HttpPost, ActionName("List")]
   [FormValueRequired("plugin-discard-changes")]
   public virtual async Task<IActionResult> DiscardChanges()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
         return AccessDeniedView();

      _pluginService.ResetChanges();

      return RedirectToAction("List");
   }

   public virtual async Task<IActionResult> EditPopup(string systemName)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
         return AccessDeniedView();

      //try to get a plugin with the specified system name
      var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.All);
      if (pluginDescriptor == null)
         return RedirectToAction("List");

      //prepare model
      var model = await _pluginModelFactory.PreparePluginModelAsync(null, pluginDescriptor);

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> EditPopup(PluginModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
         return AccessDeniedView();

      //try to get a plugin with the specified system name
      var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(model.SystemName, LoadPluginsMode.All);
      if (pluginDescriptor == null)
         return RedirectToAction("List");

      if (ModelState.IsValid)
      {
         ViewBag.RefreshPage = true;

         //we allow editing of 'friendly name', 'display order'
         pluginDescriptor.FriendlyName = model.FriendlyName;
         pluginDescriptor.DisplayOrder = model.DisplayOrder;
         
         pluginDescriptor.LimitedToUserRoles.Clear();
         if (model.SelectedUserRoleIds.Any())
            pluginDescriptor.LimitedToUserRoles = model.SelectedUserRoleIds;

         //update the description file
         pluginDescriptor.Save();

         //raise event
         await _eventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

         //locales
         var pluginInstance = pluginDescriptor.Instance<IPlugin>();
         foreach (var localized in model.Locales)
            await _localizationService.SaveLocalizedFriendlyNameAsync(pluginInstance, localized.LanguageId, localized.FriendlyName);

         //enabled/disabled
         if (!pluginDescriptor.Installed)
            return View(model);

         bool pluginIsActive;
         switch (pluginInstance)
         {
            case IExternalAuthenticationMethod externalAuthenticationMethod:
               pluginIsActive = _authenticationPluginManager.IsPluginActive(externalAuthenticationMethod);
               if (pluginIsActive && !model.IsEnabled)
               {
                  //mark as disabled
                  _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(pluginDescriptor.SystemName);
                  await _settingService.SaveSettingAsync(_externalAuthenticationSettings);
                  break;
               }

               if (!pluginIsActive && model.IsEnabled)
               {
                  //mark as enabled
                  _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(pluginDescriptor.SystemName);
                  await _settingService.SaveSettingAsync(_externalAuthenticationSettings);
               }

               break;
            case IMultiFactorAuthenticationMethod multiFactorAuthenticationMethod:
               pluginIsActive = _multiFactorAuthenticationPluginManager.IsPluginActive(multiFactorAuthenticationMethod);
               if (pluginIsActive && !model.IsEnabled)
               {
                  //mark as disabled
                  _multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(pluginDescriptor.SystemName);
                  await _settingService.SaveSettingAsync(_multiFactorAuthenticationSettings);
                  break;
               }

               if (!pluginIsActive && model.IsEnabled)
               {
                  //mark as enabled
                  _multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(pluginDescriptor.SystemName);
                  await _settingService.SaveSettingAsync(_multiFactorAuthenticationSettings);
               }

               break;
            case IWidgetPlugin widgetPlugin:
               pluginIsActive = _widgetPluginManager.IsPluginActive(widgetPlugin);
               if (pluginIsActive && !model.IsEnabled)
               {
                  //mark as disabled
                  _widgetSettings.ActiveWidgetSystemNames.Remove(pluginDescriptor.SystemName);
                  await _settingService.SaveSettingAsync(_widgetSettings);
                  break;
               }

               if (!pluginIsActive && model.IsEnabled)
               {
                  //mark as enabled
                  _widgetSettings.ActiveWidgetSystemNames.Add(pluginDescriptor.SystemName);
                  await _settingService.SaveSettingAsync(_widgetSettings);
               }

               break;
         }

         //activity log
         await _userActivityService.InsertActivityAsync("EditPlugin",
             string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditPlugin"), pluginDescriptor.FriendlyName));

         return View(model);
      }

      //prepare model
      model = await _pluginModelFactory.PreparePluginModelAsync(model, pluginDescriptor, true);

      //if we got this far, something failed, redisplay form
      return View(model);
   }

   public virtual async Task<IActionResult> OfficialFeed()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
         return AccessDeniedView();

      //prepare model
      var model = await _pluginModelFactory.PrepareOfficialFeedPluginSearchModelAsync(new OfficialFeedPluginSearchModel());

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> OfficialFeedSelect(OfficialFeedPluginSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _pluginModelFactory.PrepareOfficialFeedPluginListModelAsync(searchModel);

      return Json(model);
   }

   #endregion
}