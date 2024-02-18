﻿using System.Collections.Generic;
using Hub.Core.Domain.Security;

namespace Hub.Services.Security
{
   /// <summary>
   /// Permission provider
   /// </summary>
   public interface IPermissionProvider
   {
      /// <summary>
      /// Get permissions
      /// </summary>
      /// <returns>Permissions</returns>
      IEnumerable<PermissionRecord> GetPermissions();

      /// <summary>
      /// Get default permissions
      /// </summary>
      /// <returns>Default permissions</returns>
      HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions();
   }
}
