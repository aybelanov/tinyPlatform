using System;
using System.Linq;
using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Security;
using Hub.Core.Domain.Users;
using Hub.Services.Security;
using Hub.Services.Users;
using Hub.Web.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Clients.Configuration;
using Shared.Common;

namespace Hub.Web.Framework.Factories;

/// <summary>
/// Represents the base implementation of the factory of model which supports access control list (ACL)
/// </summary>
public partial class AclSupportedModelFactory : IAclSupportedModelFactory
{
   #region Fields

   private readonly IAclService _aclService;
   private readonly IUserService _userService;

   #endregion

   #region Ctor

   /// <summary>IoC Ctor</summary>
   public AclSupportedModelFactory(IAclService aclService,
       IUserService userService)
   {
      _aclService = aclService;
      _userService = userService;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare selected and all available user roles for the passed model
   /// </summary>
   /// <typeparam name="TModel">ACL supported model type</typeparam>
   /// <param name="model">Model</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task PrepareModelUserRolesAsync<TModel>(TModel model) where TModel : IAclSupportedModel
   {
      if (model == null)
         throw new ArgumentNullException(nameof(model));

      //prepare available user roles
      var availableRoles = (await _userService.GetAllUserRolesAsync(showHidden: true)).Where(x => x.SystemName != UserDefaults.DevicesRoleName);
      model.AvailableUserRoles = availableRoles.Select(role => new SelectListItem
      {
         Text = role.Name,
         Value = role.Id.ToString(),
         Selected = model.SelectedUserRoleIds.Contains(role.Id)
      }).ToList();
   }

   /// <summary>
   /// Prepare selected and all available user roles for the passed model by ACL mappings
   /// </summary>
   /// <typeparam name="TModel">ACL supported model type</typeparam>
   /// <typeparam name="TEntity">ACL supported entity type</typeparam>
   /// <param name="model">Model</param>
   /// <param name="entity">Entity</param>
   /// <param name="ignoreAclMappings">Whether to ignore existing ACL mappings</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   public virtual async Task PrepareModelUserRolesAsync<TModel, TEntity>(TModel model, TEntity entity, bool ignoreAclMappings)
       where TModel : IAclSupportedModel where TEntity : BaseEntity, IAclSupported
   {
      if (model == null)
         throw new ArgumentNullException(nameof(model));

      //prepare user roles with granted access
      if (!ignoreAclMappings && entity != null)
         model.SelectedUserRoleIds = (await _aclService.GetUserRoleIdsWithAccessAsync(entity)).ToList();

      await PrepareModelUserRolesAsync(model);
   }

   #endregion
}