using System.Threading.Tasks;
using Hub.Core;
using Hub.Core.Domain.Security;
using Hub.Web.Framework.Models;
using Shared.Common;

namespace Hub.Web.Framework.Factories;

/// <summary>
/// Represents the factory of model which supports access control list (ACL)
/// </summary>
public partial interface IAclSupportedModelFactory
{
   /// <summary>
   /// Prepare selected and all available user roles for the passed model
   /// </summary>
   /// <typeparam name="TModel">ACL supported model type</typeparam>
   /// <param name="model">Model</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task PrepareModelUserRolesAsync<TModel>(TModel model) where TModel : IAclSupportedModel;

   /// <summary>
   /// Prepare selected and all available user roles for the passed model by ACL mappings
   /// </summary>
   /// <typeparam name="TModel">ACL supported model type</typeparam>
   /// <typeparam name="TEntity">ACL supported entity type</typeparam>
   /// <param name="model">Model</param>
   /// <param name="entity">Entity</param>
   /// <param name="ignoreAclMappings">Whether to ignore existing ACL mappings</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task PrepareModelUserRolesAsync<TModel, TEntity>(TModel model, TEntity entity, bool ignoreAclMappings)
       where TModel : IAclSupportedModel where TEntity : BaseEntity, IAclSupported;
}