using Hub.Web.Areas.Admin.Models.Users;
using Hub.Web.Framework.Models;
using System.Collections.Generic;

namespace Hub.Web.Areas.Admin.Models.Security
{
   /// <summary>
   /// Represents a permission mapping model
   /// </summary>
   public partial record PermissionMappingModel : BaseAppModel
   {
      #region Ctor

      public PermissionMappingModel()
      {
         AvailablePermissions = new List<PermissionRecordModel>();
         AvailableUserRoles = new List<UserRoleModel>();
         Allowed = new Dictionary<string, IDictionary<long, bool>>();
      }

      #endregion

      #region Properties

      public IList<PermissionRecordModel> AvailablePermissions { get; set; }

      public IList<UserRoleModel> AvailableUserRoles { get; set; }

      //[permission system name] / [user role id] / [allowed]
      public IDictionary<string, IDictionary<long, bool>> Allowed { get; set; }

      #endregion
   }
}