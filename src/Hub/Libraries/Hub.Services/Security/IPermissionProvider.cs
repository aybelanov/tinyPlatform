﻿using Hub.Core.Domain.Security;
using System.Collections.Generic;

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
