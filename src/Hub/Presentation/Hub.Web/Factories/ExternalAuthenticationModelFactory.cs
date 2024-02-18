using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub.Web.Models.User;
using Hub.Core;
using Hub.Services.Authentication.External;

namespace Hub.Web.Factories;

/// <summary>
/// Represents the external authentication model factory
/// </summary>
public partial class ExternalAuthenticationModelFactory : IExternalAuthenticationModelFactory
{
   #region Fields

   private readonly IAuthenticationPluginManager _authenticationPluginManager;
   private readonly IWorkContext _workContext;

   #endregion

   #region Ctor

   public ExternalAuthenticationModelFactory(IAuthenticationPluginManager authenticationPluginManager, IWorkContext workContext)
   {
      _authenticationPluginManager = authenticationPluginManager;
      _workContext = workContext;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare the external authentication method model
   /// </summary>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the list of the external authentication method model
   /// </returns>
   public virtual async Task<List<ExternalAuthenticationMethodModel>> PrepareExternalMethodsModelAsync()
   {
      return (await _authenticationPluginManager
          .LoadActivePluginsAsync(await _workContext.GetCurrentUserAsync()))
          .Select(authenticationMethod => new ExternalAuthenticationMethodModel
          {
             ViewComponentName = authenticationMethod.GetPublicViewComponentName()
          })
          .ToList();
   }

   #endregion
}