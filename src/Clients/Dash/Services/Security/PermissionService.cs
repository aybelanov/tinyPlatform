using Clients.Dash.Configuration;
using Clients.Dash.Services.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Clients.Configuration;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Clients.Dash.Services.Security;

/// <summary>
/// Represents a admin mode service implementation
/// </summary>
public class PermissionService
{
   #region fields

   private readonly SettingsService _settingsService;
   private readonly AuthenticationStateProvider _authenticationStateProvider;

   bool? _cashedEnabledValue;

   #endregion

   #region Ctors

   /// <summary>
   /// IoC Ctor
   /// </summary>
   public PermissionService(SettingsService settingsService, AuthenticationStateProvider authenticationStateProvider)
   {
      _settingsService = settingsService;
      _authenticationStateProvider = authenticationStateProvider;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Check admin mode
   /// </summary>
   /// <param name="state">Athentication state</param>
   /// <returns></returns>
   public bool IsAdminMode(AuthenticationState state)
   {
      if (state?.User?.IsInRole(UserDefaults.AdministratorsRoleName) ?? false)
      {
         if (!_cashedEnabledValue.HasValue)
         {
            var mode = _settingsService.GetSettingByKey<bool>(Defaults.IsAdminModeEnabled);
            _cashedEnabledValue = mode;
         }

         return _cashedEnabledValue.Value;
      }

      return false;
   }

   /// <summary>
   /// Check admin mode
   /// </summary>
   /// <returns>Async opertaion</returns>
   public async Task<bool> IsAdminModeAsync()
   {
      var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
      if (authState?.User?.IsInRole(UserDefaults.AdministratorsRoleName) ?? false)
      {
         if (!_cashedEnabledValue.HasValue)
         {
            var mode = await _settingsService.GetSettingByKeyAsync<bool>(Defaults.IsAdminModeEnabled);
            _cashedEnabledValue = mode;
         }

         return _cashedEnabledValue.Value;
      }

      return false;
   }

   /// <summary>
   /// Gets current user
   /// </summary>
   /// <returns></returns>
   public async Task<ClaimsPrincipal> GetCurentUserAsync()
   {
      var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
      return authState.User;
   }

   /// <summary>
   /// Sets admin mode
   /// </summary>
   /// <param name="isAdminModeEnabled">Admin mode state</param>
   /// <returns>Async opertaion</returns>
   public async Task SetModeAsync(bool isAdminModeEnabled)
   {
      var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
      if (authState?.User?.IsInRole(UserDefaults.AdministratorsRoleName) ?? false)
      {
         await _settingsService.SetOrUpdateSettingAsync(Defaults.IsAdminModeEnabled, isAdminModeEnabled);
         _cashedEnabledValue = isAdminModeEnabled;
         AdminModeChanged?.Invoke(null, isAdminModeEnabled);
      }
   }

   /// <summary>
   /// Sets admin mode
   /// </summary>
   /// <param name="isAdminModeEnabled">Admin mode state</param>
   /// <param name="state">Athentication state</param>
   /// <returns></returns>
   public void SetMode(bool isAdminModeEnabled, AuthenticationState state)
   {
      if (state?.User?.IsInRole(UserDefaults.AdministratorsRoleName) ?? false)
      {
         _settingsService.SetOrUpdateSetting(Defaults.IsAdminModeEnabled, isAdminModeEnabled);
         _cashedEnabledValue = isAdminModeEnabled;
         AdminModeChanged?.Invoke(null, isAdminModeEnabled);
      }
   }

   #endregion

   #region Events

   /// <summary>
   /// Admin mode changed event
   /// </summary>
   public event EventHandler<bool> AdminModeChanged;

   #endregion
}
