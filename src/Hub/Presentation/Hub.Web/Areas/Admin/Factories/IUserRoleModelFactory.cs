using Hub.Core.Domain.Users;
using Hub.Web.Areas.Admin.Models.Users;
using System.Threading.Tasks;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the user role model factory
/// </summary>
public partial interface IUserRoleModelFactory
{
   /// <summary>
   /// Prepare user role search model
   /// </summary>
   /// <param name="searchModel">User role search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user role search model
   /// </returns>
   Task<UserRoleSearchModel> PrepareUserRoleSearchModelAsync(UserRoleSearchModel searchModel);

   /// <summary>
   /// Prepare paged user role list model
   /// </summary>
   /// <param name="searchModel">User role search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user role list model
   /// </returns>
   Task<UserRoleListModel> PrepareUserRoleListModelAsync(UserRoleSearchModel searchModel);

   /// <summary>
   /// Prepare user role model
   /// </summary>
   /// <param name="model">User role model</param>
   /// <param name="userRole">User role</param>
   /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user role model
   /// </returns>
   Task<UserRoleModel> PrepareUserRoleModelAsync(UserRoleModel model, UserRole userRole, bool excludeProperties = false);
}