using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core.Domain.Users;
using Hub.Services.Users;
using Hub.Services.Plugins;

namespace Hub.Services.Authentication.MultiFactor;

/// <summary>
/// Represents an multi-factor authentication plugin manager implementation
/// </summary>
public partial class MultiFactorAuthenticationPluginManager : PluginManager<IMultiFactorAuthenticationMethod>, IMultiFactorAuthenticationPluginManager
{
   #region Fields

   private readonly MultiFactorAuthenticationSettings _multiFactorAuthenticationSettings;

   #endregion

   #region Ctor

   /// <summary>
   /// Ioc Ctor
   /// </summary>
   /// <param name="multiFactorAuthenticationSettings"></param>
   /// <param name="userService"></param>
   /// <param name="pluginService"></param>
   public MultiFactorAuthenticationPluginManager(MultiFactorAuthenticationSettings multiFactorAuthenticationSettings,
       IUserService userService,
       IPluginService pluginService) : base(userService, pluginService)
   {
      _multiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Check is active multi-factor authentication methods
   /// </summary>
   /// <param name="user">Filter by user; pass null to load all plugins</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the rue - if active multi-factor authentication methods
   /// </returns>
   public virtual async Task<bool> HasActivePluginsAsync(User user = null)
   {
      return (await LoadActivePluginsAsync(_multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames, user)).Any();
   }

   /// <summary>
   /// Load active multi-factor authentication methods
   /// </summary>
   /// <param name="user">Filter by user; pass null to load all plugins</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the list of active multi-factor authentication methods
   /// </returns>
   public virtual async Task<IList<IMultiFactorAuthenticationMethod>> LoadActivePluginsAsync(User user = null)
   {
      return await LoadActivePluginsAsync(_multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames, user);
   }

   /// <summary>
   /// Check whether the passed multi-factor authentication method is active
   /// </summary>
   /// <param name="authenticationMethod">Authentication method to check</param>
   /// <returns>Result</returns>
   public virtual bool IsPluginActive(IMultiFactorAuthenticationMethod authenticationMethod)
   {
      return IsPluginActive(authenticationMethod, _multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames);
   }

   /// <summary>
   /// Check whether the multi-factor authentication method with the passed system name is active
   /// </summary>
   /// <param name="systemName">System name of authentication method to check</param>
   /// <param name="user">Filter by user; pass null to load all plugins</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the result
   /// </returns>
   public virtual async Task<bool> IsPluginActiveAsync(string systemName, User user = null)
   {
      var authenticationMethod = await LoadPluginBySystemNameAsync(systemName, user);
      return IsPluginActive(authenticationMethod);
   }

   #endregion

}
