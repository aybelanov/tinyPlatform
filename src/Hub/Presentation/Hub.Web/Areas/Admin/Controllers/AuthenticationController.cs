using System.Threading.Tasks;
using Hub.Core.Domain.Users;
using Hub.Core.Events;
using Hub.Services.Authentication.External;
using Hub.Services.Authentication.MultiFactor;
using Hub.Services.Configuration;
using Hub.Services.Plugins;
using Hub.Services.Security;
using Hub.Web.Areas.Admin.Factories;
using Hub.Web.Areas.Admin.Models.ExternalAuthentication;
using Hub.Web.Areas.Admin.Models.MultiFactorAuthentication;
using Hub.Web.Framework.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Areas.Admin.Controllers;

public partial class AuthenticationController : BaseAdminController
{
   #region Fields

   private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
   private readonly IAuthenticationPluginManager _authenticationPluginManager;
   private readonly IEventPublisher _eventPublisher;
   private readonly IExternalAuthenticationMethodModelFactory _externalAuthenticationMethodModelFactory;
   private readonly IMultiFactorAuthenticationMethodModelFactory _multiFactorAuthenticationMethodModelFactory;
   private readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
   private readonly IPermissionService _permissionService;
   private readonly ISettingService _settingService;
   private readonly MultiFactorAuthenticationSettings _multiFactorAuthenticationSettings;

   #endregion

   #region Ctor

   public AuthenticationController(ExternalAuthenticationSettings externalAuthenticationSettings,
       IAuthenticationPluginManager authenticationPluginManager,
       IEventPublisher eventPublisher,
       IExternalAuthenticationMethodModelFactory externalAuthenticationMethodModelFactory,
       IMultiFactorAuthenticationMethodModelFactory multiFactorAuthenticationMethodModelFactory,
       IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
       IPermissionService permissionService,
       ISettingService settingService,
       MultiFactorAuthenticationSettings multiFactorAuthenticationSettings)
   {
      _externalAuthenticationSettings = externalAuthenticationSettings;
      _authenticationPluginManager = authenticationPluginManager;
      _eventPublisher = eventPublisher;
      _externalAuthenticationMethodModelFactory = externalAuthenticationMethodModelFactory;
      _multiFactorAuthenticationMethodModelFactory = multiFactorAuthenticationMethodModelFactory;
      _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
      _permissionService = permissionService;
      _settingService = settingService;
      _multiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
   }

   #endregion

   #region External Authentication

   public virtual async Task<IActionResult> ExternalMethods()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
         return AccessDeniedView();

      //prepare model
      var model = _externalAuthenticationMethodModelFactory
          .PrepareExternalAuthenticationMethodSearchModel(new ExternalAuthenticationMethodSearchModel());

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> ExternalMethods(ExternalAuthenticationMethodSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _externalAuthenticationMethodModelFactory.PrepareExternalAuthenticationMethodListModelAsync(searchModel);

      return Json(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> ExternalMethodUpdate(ExternalAuthenticationMethodModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
         return AccessDeniedView();

      var method = await _authenticationPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
      if (_authenticationPluginManager.IsPluginActive(method))
         if (!model.IsActive)
         {
            //mark as disabled
            _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(method.PluginDescriptor.SystemName);
            await _settingService.SaveSettingAsync(_externalAuthenticationSettings);
         }
      else
         if (model.IsActive)
      {
         //mark as active
         _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(method.PluginDescriptor.SystemName);
         await _settingService.SaveSettingAsync(_externalAuthenticationSettings);
      }

      var pluginDescriptor = method.PluginDescriptor;
      pluginDescriptor.DisplayOrder = model.DisplayOrder;

      //update the description file
      pluginDescriptor.Save();

      //raise event
      await _eventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

      return new NullJsonResult();
   }

   #endregion

   #region Multi-factor Authentication

   public virtual async Task<IActionResult> MultiFactorMethods()
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMultifactorAuthenticationMethods))
         return AccessDeniedView();

      //prepare model
      var model = _multiFactorAuthenticationMethodModelFactory
          .PrepareMultiFactorAuthenticationMethodSearchModel(new MultiFactorAuthenticationMethodSearchModel());

      return View(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> MultiFactorMethods(MultiFactorAuthenticationMethodSearchModel searchModel)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMultifactorAuthenticationMethods))
         return await AccessDeniedDataTablesJson();

      //prepare model
      var model = await _multiFactorAuthenticationMethodModelFactory.PrepareMultiFactorAuthenticationMethodListModelAsync(searchModel);

      return Json(model);
   }

   [HttpPost]
   public virtual async Task<IActionResult> MultiFactorMethodUpdate(MultiFactorAuthenticationMethodModel model)
   {
      if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMultifactorAuthenticationMethods))
         return AccessDeniedView();

      var method = await _multiFactorAuthenticationPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
      if (_multiFactorAuthenticationPluginManager.IsPluginActive(method))
         if (!model.IsActive)
         {
            //mark as disabled
            _multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(method.PluginDescriptor.SystemName);
            await _settingService.SaveSettingAsync(_multiFactorAuthenticationSettings);
         }
      else
         if (model.IsActive)
      {
         //mark as active
         _multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(method.PluginDescriptor.SystemName);
         await _settingService.SaveSettingAsync(_multiFactorAuthenticationSettings);
      }

      var pluginDescriptor = method.PluginDescriptor;
      pluginDescriptor.DisplayOrder = model.DisplayOrder;

      //update the description file
      pluginDescriptor.Save();

      //raise event
      await _eventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

      return new NullJsonResult();
   }

   #endregion
}